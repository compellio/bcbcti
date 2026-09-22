using Compellio.Bcbcti.Models.Stix;
using Compellio.Bcbcti.Services;
using Compellio.Bcbcti.Services.Taxii.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Compellio.Bcbcti.Controllers;

[ApiController]
[Route("/api/collections/{collectionId}/objects/{objectId}")]
[Consumes("application/taxii+json", "application/taxii+json;version=2.1")]
[Produces("application/taxii+json;version=2.1")]
public class ObjectController(CollectionsManager collections) : ControllerBase
{
    
    [HttpGet(Name = "GetObject")]
    public StixObject Get(string collectionId, string objectId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);
        
        // TODO return STIX object resource + append extra registration metadata?
        throw new NotImplementedException();
    }
    
    [HttpDelete(Name = "DeleteObject")]
    public StixObject Delete(string collectionId, string objectId)
    {
        throw new UnsupportedFeatureException("Registered object deletion is not supported");
    }

    [HttpGet(Name = "ListObjectVersions")]
    [Route("/versions")]
    public StixObject ListVersions(string collectionId, string objectId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);
        
        // TODO return list of object versions + extra registration metadata?
        throw new NotImplementedException();
    }

}