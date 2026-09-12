using Bcbcti.Exceptions.Taxii;
using Bcbcti.Models.Stix;
using Bcbcti.Models.Taxii;
using Bcbcti.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bcbcti.Controllers;

[ApiController]
[Route("/api/collections/{collectionId}")]
[Consumes("application/taxii+json", "application/taxii+json;version=2.1")]
[Produces("application/taxii+json;version=2.1")]
public class CollectionController(CollectionsCatalog collections) : ControllerBase
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

    [HttpPost(Name = "CreateObjects")]
    [Route("/objects")]
    public StixObjectResource CreateObjects(string collectionId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);
        
        // TODO process input and pass on individual objects to the submission handler
        throw new NotImplementedException();
    }
}