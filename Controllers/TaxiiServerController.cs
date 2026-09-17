using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Controllers;

[ApiController]
[Route("/")]
[Consumes("application/taxii+json", "application/taxii+json;version=2.1")]
[Produces("application/taxii+json;version=2.1")]
public class TaxiiServerController(IOptions<TaxiiOptions> options) : ControllerBase
{
    [HttpGet(Name = "TaxiiServerDiscovery")]
    [Route("/taxii2")]
    public DiscoveryResource Discovery()
    {
        return new DiscoveryResource
        {
            Title = options.Value.ServerTitle, Default = "/api/", ApiRoots = ["/api/"]
        };
    }

    [HttpGet(Name = "TaxiiApiRootInformation")]
    [Route("/api")]
    public ApiRootResource RootInformation()
    {
        return new ApiRootResource
        {
            Title = options.Value.Title,
            Description = options.Value.Description,
            Versions = ["application/taxii+json;version=2.1"],
            MaxContentLength = (int)options.Value.MaxUploadBytes,
        };
    }
    
    // TODO /{api-root}/status/{status-id}
}