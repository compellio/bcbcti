using Compellio.Bcbcti.Extensions;
using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Models.Stix;
using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services.Ingestion;
using Compellio.Bcbcti.Services.Taxii.Exceptions;
using Compellio.Bcbcti.Services.Taxii.Mappers;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.Taxii;

public class CollectionsService
{
    private readonly IOptions<TaxiiOptions> _taxiiOptions;

    private readonly CollectionsManager _collections;
    private readonly StixIngestionService _stixIngestionService;
    private readonly JournalRepository _journalRepository;
    private readonly ManifestRepository _manifestRepository;
    private readonly StixObjectRepository _stixObjectRepository;

    public CollectionsService(IOptions<TaxiiOptions> taxiiOptions, CollectionsManager collections,
        StixIngestionService stixIngestionService, JournalRepository journalRepository,
        ManifestRepository manifestRepository, StixObjectRepository stixObjectRepository)
    {
        _taxiiOptions = taxiiOptions;

        _collections = collections;
        _stixIngestionService = stixIngestionService;
        _journalRepository = journalRepository;
        _manifestRepository = manifestRepository;
        _stixObjectRepository = stixObjectRepository;
    }

    public CollectionResource GetCollection(string collectionId)
    {
        var collection = _collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);

        return CollectionMapper.ToResource(collection, true, true);
    }

    public async Task<EnvelopeResource> GetObjectsEnvelope(DateTime? addedAfter, int? limit,
        CancellationToken ct = default)
    {
        // TODO FIXME duplication
        var take = Math.Min(limit ?? _taxiiOptions.Value.Pagination.DefaultLimit,
            _taxiiOptions.Value.Pagination.MaxLimit);

        var page = await _manifestRepository.GetManifestPage(take, addedAfter, ct);

        // TODO set ParallelOptions.MaxDegreeOfParallelism based on current store options (e.g. AmazonS3Config.MaxConnectionsPerServer, default = 50)
        var entries = await page.Items.ParallelSelectAsync(async (manifestObject, ctoken) =>
        {
            var manifest = await _manifestRepository.GetManifestEntry(manifestObject.ObjectKey, ctoken);
            var stixObject = await _stixObjectRepository.GetStixObject(manifest.Body.ObjectKey, ctoken);

            return (StixObject: stixObject.Body, CompletedAt: manifest.Body.CompletedAt);
        }, ct);

        DateTime? firstAdded = entries.Count > 0 ? entries[0].CompletedAt : null;
        DateTime? lastAdded = entries.Count > 0 ? entries[^1].CompletedAt : null;

        return EnvelopeMapper.ToResource(entries.Select(entry => entry.StixObject).ToArray(), page.HasMore, firstAdded,
            lastAdded);
    }

    public async Task<StatusResource> SubmitObjects(CollectionOptions collection, DateTime submittedAt,
        StixObject[] objects, CancellationToken ct = default)
    {
        var journalId = Guid.NewGuid();
        var results = await _stixIngestionService.ProcessStixObjects(collection, journalId, submittedAt, objects, ct);

        var journalEntry = new JournalEntry
        {
            Id = journalId,
            CollectionId = collection.Id,
            RequestTimestamp = submittedAt,
            Objects = results.Select(result => new JournalEntry.Object
                {
                    ObjectId = result.StixObject.Id,
                    ObjectVersion = result.StixObject.Version(submittedAt), // TODO FIXME duplication read from result
                    ObjectKey = result.RegistrationReceipt?.ObjectKey,
                    ReceiptId = result.RegistrationReceipt?.ReceiptId,
                    SubmitFailureReason = result.Resolution switch
                    {
                        // TODO FIXME hardcoded error -> new enum/static consts for user error codes?
                        IngestionResultResolution.Abort => "pending registration",
                        IngestionResultResolution.Failure => result.ResolutionFailureMessage,
                        _ => null
                    }
                })
                .ToArray()
        };

        await _journalRepository.PutJournalEntry(journalEntry, ct);

        var receipts = results.Select(result => result.RegistrationReceipt)
            .WhereNotNull()
            .ToDictionary(r => r.ReceiptId);

        return StatusMapper.ToResource(journalEntry, receipts);
    }
}