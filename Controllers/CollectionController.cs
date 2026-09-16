using Bcbcti.Exceptions.Taxii;
using Bcbcti.Models.Documents;
using Bcbcti.Models.Stix;
using Bcbcti.Models.Taxii;
using Bcbcti.Models.Taxii.Requests;
using Bcbcti.Repositories;
using Bcbcti.Services;
using Bcbcti.Services.Ingestion;
using Microsoft.AspNetCore.Mvc;

namespace Bcbcti.Controllers;

[ApiController]
[Route("/api/collections/{collectionId}")]
[Consumes("application/taxii+json", "application/taxii+json;version=2.1")]
[Produces("application/taxii+json;version=2.1")]
public class CollectionController(CollectionsManager collections) : ControllerBase
{
    [HttpGet(Name = "GetCollection")]
    public CollectionResource Get(string collectionId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);

        return CollectionResource.FromCollectionOptions(collection, true, true);
    }

    [HttpGet(Name = "ListManifests")]
    [Route("/manifest")]
    public ManifestResource ListManifests(string collectionId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);

        // TODO return registry-API status/data for submitted objects + apply filtering
        throw new NotImplementedException();
    }

    [HttpGet(Name = "ListObjects")]
    [Route("/objects")]
    public EnvelopeResource ListObjects(string collectionId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);

        // TODO return submitted objects + apply filtering
        throw new NotImplementedException();
    }

    /// <summary>
    /// TAXII 5.5 Add Objects endpoint
    /// </summary>
    /// <see href="https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285815"/>
    [HttpPost(Name = "CreateObjects")]
    [Route("/objects")]
    public async Task<StixObject> CreateObjects(string collectionId, [FromBody] AddObjectsRequest envelope,
        [FromServices] JournalRepository journalRepository, [FromServices] StixIngestionService stixIngestionService,
        CancellationToken ct)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);

        var statusId = Guid.NewGuid();
        var submittedAt = DateTime.UtcNow;

        var results = await stixIngestionService.ProcessStixObjects(envelope.Objects, submittedAt, ct);
        
        // 3. create journal entry with receipt ids
        await journalRepository.PutJournalEntry(new JournalEntry
        {
            Id = statusId,
            CollectionId = collection.Id,
            RequestTimestamp = submittedAt,
            Objects = results.Select(result => new JournalEntryObject
                {
                    Id = $"indicator--{Guid.NewGuid()}",
                    ObjectKey = result.ObjectKey,
                    ReceiptId = result.ReceiptId
                })
                .ToArray()
        });

        // 4. generate status and return

        // TODO return same response as /status/{status-id} => constructor/factory?

        throw new NotImplementedException();
    }
}