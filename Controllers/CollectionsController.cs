using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Services;
using Compellio.Bcbcti.Services.Taxii.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace Compellio.Bcbcti.Controllers;

[ApiController]
[Route("/api/collections")]
[Consumes("application/taxii+json", "application/taxii+json;version=2.1")]
[Produces("application/taxii+json;version=2.1")]
public class CollectionsController : ControllerBase
{
    [HttpGet(Name = "ListCollections")]
    public CollectionsResource Index(CollectionsManager collections)
    {
        return CollectionsMapper.ToResource(collections.All);
    }
}