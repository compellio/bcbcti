// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Services.Ingestion.Registry;
using Compellio.Bcbcti.Services.RegistryApi.Models;

namespace Compellio.Bcbcti.Services.RegistryApi;

public interface IRegistryApiClient
{
    public Task<RegistryResponse> RegisterTarPayload(TarPayload tarPayload, CancellationToken ct = default);

    public Task<RegistryResponse> UpdateTarPayload(string tarId, TarPayload tarPayload, CancellationToken ct = default);

    /// <summary>
    /// Retrieve a TAR by its id
    /// </summary>
    /// <remarks>Calls GET /api/v1/TAR/{tarID}</remarks>
    /// <returns></returns>
    public Task<RegistryResponse> GetTar(string tarId, CancellationToken ct = default);

    /// <summary>
    /// Retrive a TAR by a receipt id
    /// </summary>
    /// <remarks>Calls GET /api/v1/TAR/tarId/{receiptID}</remarks>
    /// <returns></returns>
    public Task<RegistryResponse> GetTar(Guid receiptId, CancellationToken ct = default);

    /// <summary>
    /// Verifies (webhook secret, etc.) and transforms an HTTP callback response into a RegistryResponse
    /// </summary>
    /// <returns></returns>
    public Task<RegistryResponse> ValidateCallback( /* TODO http context */);
}
