using Compellio.Bcbcti.Extensions;
using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Models.Stix;
using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services.Ingestion;
using Compellio.Bcbcti.Services.Taxii.Exceptions;
using Compellio.Bcbcti.Services.Taxii.Mappers;

namespace Compellio.Bcbcti.Services.Taxii;

public class CollectionsService
{
    private readonly CollectionsManager _collections;
    private readonly StixIngestionService _stixIngestionService;
    private readonly JournalRepository _journalRepository;

    public CollectionsService(CollectionsManager collections, StixIngestionService stixIngestionService,
        JournalRepository journalRepository)
    {
        _collections = collections;
        _stixIngestionService = stixIngestionService;
        _journalRepository = journalRepository;
    }

    public CollectionResource GetCollection(string collectionId)
    {
        var collection = _collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);

        return CollectionMapper.ToResource(collection, true, true);
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