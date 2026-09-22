using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Models.Taxii.Requests;
using Compellio.Bcbcti.Services;
using Compellio.Bcbcti.Services.Taxii;
using Compellio.Bcbcti.Services.Taxii.Exceptions;
using Compellio.Bcbcti.Services.Taxii.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace Compellio.Bcbcti.Controllers;

[ApiController]
[Route("api/collections/{collectionId}")]
[Consumes("application/taxii+json", "application/taxii+json;version=2.1")]
[Produces("application/taxii+json;version=2.1")]
public class CollectionController(CollectionsManager collections) : ControllerBase
{
    [HttpGet(Name = "GetCollection")]
    public CollectionResource Get(string collectionId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);
        
        return CollectionMapper.ToResource(collection, true, true);
    }

    [HttpGet("manifest", Name = "ListManifests")]
    public async Task<ManifestResource> ListManifests(string collectionId, [FromQuery] FilteringParameters filters,
        ManifestService service, CancellationToken ct)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);
        
        // TODO throw unsupported if match in filters
        
        return await service.GetManifest(filters.AddedAfter, filters.Limit, ct);
    }

    [HttpGet("objects", Name = "ListObjects")]
    public async Task<EnvelopeResource> ListObjects(string collectionId,  [FromQuery] FilteringParameters filters, CollectionsService service, CancellationToken ct)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);
        
        // TODO throw unsupported if match in filters
        
        return await service.GetObjectsEnvelope(filters.AddedAfter, filters.Limit, ct);
    }

    /// <summary>
    /// TAXII 5.5 Add Objects endpoint
    /// </summary>
    /// <see href="https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285815"/>
    [HttpPost("objects", Name = "CreateObjects")]
    public async Task<ActionResult<StatusResource>> CreateObjects(string collectionId,
        [FromBody] AddObjectsRequest envelope, CollectionsService service, CancellationToken ct)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);

        // TODO TAXXI spec: throw validation error if invalid STIX objects contained in envelope (normally handled by AddObjectsRequest?)
        // TODO check for duplicates in envelope? (duplicate = same id AND version)

        var submittedAt = DateTime.UtcNow;
        var status = await service.SubmitObjects(collection, submittedAt, envelope.Objects, ct);

        return Accepted(status);
    }
}