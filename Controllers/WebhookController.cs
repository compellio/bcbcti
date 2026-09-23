using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services.Ingestion;
using Compellio.Bcbcti.Services.RegistryApi;
using Microsoft.AspNetCore.Mvc;

namespace Compellio.Bcbcti.Controllers;

[ApiController]
[Route("api/hooks")]
[Consumes("application/json")]
[Produces("application/json")]
public class WebhookController(
    IRegistryApiClient registryApiClient,
    StixReconciliationService reconciliationService,
    RegistryOperationRepository operationRepository) : ControllerBase
{
    [HttpPost("registry-api/callback", Name = "RegistryApiCallback")]
    public async Task Get( /* http context */ CancellationToken ct)
    {
        var tarReceipt = await registryApiClient.ValidateCallback( /* http context */);
        var completedAt = DateTime.UtcNow; // TODO FIXME move to IRegistryApiClient

        var reconciliationResult = await reconciliationService.ReconcileTarReceipt(tarReceipt.Receipt, completedAt, ct);

        if (reconciliationResult is StixReconciliationResult.SuccessResolution resolution)
        {
            await operationRepository.DeleteRegistryOperation(resolution.ObjectId, ct);
        }
    }
}