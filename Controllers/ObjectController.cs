using Compellio.Bcbcti.Models.Stix;
using Compellio.Bcbcti.Services;
using Compellio.Bcbcti.Services.Taxii.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace Compellio.Bcbcti.Controllers;

[ApiController]
[Route("api/collections/{collectionId}/objects/{objectId}")]
[Consumes("application/taxii+json", "application/taxii+json;version=2.1")]
[Produces("application/taxii+json;version=2.1")]
public class ObjectController(CollectionsManager collections) : ControllerBase
{
    
    [HttpGet(Name = "GetObject")]
    public StixObject Get(string collectionId, string objectId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);
        
        // TODO X-TAXII-Date-Added-First
        //      The X-TAXII-Date-Added-First header is an extension header. It indicates the date_added `timestamp` of the first object of the response.
        //      The value of this header MUST be a `timestamp`.
        
        // TODO X-TAXII-Date-Added-Last
        //      The X-TAXII-Date-Added-Last header is an extension header. It indicates the date_added `timestamp` of the last object of the response.
        //      The value of this header MUST be a `timestamp`.
        
        // TODO return STIX object resource + append extra registration metadata?
        throw new NotImplementedException();
    }
    
    [HttpDelete(Name = "DeleteObject")]
    public StixObject Delete(string collectionId, string objectId)
    {
        throw new NotSupportedException("Registered object deletion is not supported");
    }

    [HttpGet("versions", Name = "ListObjectVersions")]
    public StixObject ListVersions(string collectionId, string objectId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);
        
        // TODO X-TAXII-Date-Added-First
        //      The X-TAXII-Date-Added-First header is an extension header. It indicates the date_added `timestamp` of the first object of the response.
        //      The value of this header MUST be a `timestamp`.
        
        // TODO X-TAXII-Date-Added-Last
        //      The X-TAXII-Date-Added-Last header is an extension header. It indicates the date_added `timestamp` of the last object of the response.
        //      The value of this header MUST be a `timestamp`.
        
        // TODO return list of object versions + extra registration metadata?
        throw new NotImplementedException();
    }

}