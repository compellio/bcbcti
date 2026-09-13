using Bcbcti.Exceptions.Taxii;
using Bcbcti.Models.Stix;
using Bcbcti.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bcbcti.Controllers;

[ApiController]
[Route("/api/collections/{collectionId}/objects/{objectId}")]
[Consumes("application/taxii+json", "application/taxii+json;version=2.1")]
[Produces("application/taxii+json;version=2.1")]
public class ObjectController(CollectionsManager collections) : ControllerBase
{
    
    [HttpGet(Name = "GetObject")]
    public StixObjectResource Get(string collectionId, string objectId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);
        
        // TODO return STIX object resource + append extra registration metadata?
        throw new NotImplementedException();
    }
    
    [HttpDelete(Name = "DeleteObject")]
    public StixObjectResource Delete(string collectionId, string objectId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);
        
        // TODO delete object id
        throw new NotImplementedException();
    }
    
    [HttpGet(Name = "ListObjectVersions")]
    [Route("/versions")]
    public StixObjectResource ListVersions(string collectionId, string objectId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);
        
        // TODO return list of object versions + extra registration metadata?
        throw new NotImplementedException();
    }

}