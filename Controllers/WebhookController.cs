// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

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
    StixReconciliationService reconciliationService) : ControllerBase
{
    [HttpPost("registry-api/callback", Name = "RegistryApiCallback")]
    public async Task Get( /* http context */ CancellationToken ct)
    {
        var tarReceipt = await registryApiClient.ValidateCallback( /* http context */);
        await reconciliationService.ReconcileRegistryResponse(tarReceipt, ct);
    }
}
