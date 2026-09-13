using Bcbcti.Models.Taxii;
using Bcbcti.Services;
using Microsoft.AspNetCore.Mvc;

namespace Bcbcti.Controllers;

[ApiController]
[Route("/api/collections")]
[Consumes("application/taxii+json", "application/taxii+json;version=2.1")]
[Produces("application/taxii+json;version=2.1")]
public class CollectionsController(CollectionsManager collections) : ControllerBase
{
    [HttpGet(Name = "ListCollections")]
    public CollectionsResource Index()
    {
        return new CollectionsResource()
        {
            Collections = collections.All
                .Select(collection => CollectionResource.FromCollectionOptions(collection, true, true))
                .ToArray()
        };
    }
}