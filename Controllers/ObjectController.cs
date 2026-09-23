using Compellio.Bcbcti.Models.Stix;
using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Models.Taxii.Requests;
using Compellio.Bcbcti.Services;
using Compellio.Bcbcti.Services.Serialization.Primitives;
using Compellio.Bcbcti.Services.Taxii;
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
    public StixObject Get(string collectionId, string objectId, [FromQuery] FilteringParameters filters)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);

        UnsupportedFilteringException.ThrowIfMatchPresent(filters);

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
        throw new UnsupportedTaxiiFeatureException("Registered object deletion is not supported");
    }

    [HttpGet("versions", Name = "ListObjectVersions")]
    public async Task<Envelope<StixTimestamp>> ListVersions(string collectionId, string objectId,
        [FromQuery] FilteringParameters filters, CancellationToken ct)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);
        
        // TODO X-TAXII-Date-Added-First
        //      The X-TAXII-Date-Added-First header is an extension header. It indicates the date_added `timestamp` of the first object of the response.
        //      The value of this header MUST be a `timestamp`.
        
        // TODO X-TAXII-Date-Added-Last
        //      The X-TAXII-Date-Added-Last header is an extension header. It indicates the date_added `timestamp` of the last object of the response.
        //      The value of this header MUST be a `timestamp`.

        UnsupportedFilteringException.ThrowIfMatchPresent(filters);
        
        // TODO return list of object versions + extra registration metadata?
        throw new NotImplementedException();
    }
}