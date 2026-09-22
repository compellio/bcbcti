using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Services.Taxii;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Controllers;

[ApiController]
[Route("/")]
[Consumes("application/taxii+json", "application/taxii+json;version=2.1")]
[Produces("application/taxii+json;version=2.1")]
public class TaxiiServerController(IOptions<TaxiiOptions> options) : ControllerBase
{
    [HttpGet("taxii2", Name = "TaxiiServerDiscovery")]
    public DiscoveryResource Discovery(TaxiiServerService service)
    {
        return service.BuildDiscoveryResource();
    }

    [HttpGet("api", Name = "TaxiiApiRootInformation")]
    public ApiRootResource RootInformation(TaxiiServerService service)
    {
        return service.BuildApiRootResource();
    }

    [HttpGet("api/status/{id:guid}", Name = "TaxiiApiRootStatus")]
    public async Task<StatusResource> StatusInformation(Guid id, StatusService service, CancellationToken ct)
    {
        // TODO FIXME catch ObjectNotFoundException -> turn into TAXII 404
        return await service.BuildStatusResource(id, ct);
    }
}