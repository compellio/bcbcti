// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Services.Ingestion.Registry;
using Compellio.Bcbcti.Services.RegistryApi.Models;

namespace Compellio.Bcbcti.Services.RegistryApi;

public class MockRegistryApiClient : IRegistryApiClient
{
    private readonly ILogger<MockRegistryApiClient> _logger;
    
    public MockRegistryApiClient(ILogger<MockRegistryApiClient> logger)
    {
        _logger = logger;
    }

    private void LogRequest(string method, string path)
    {
        _logger.LogDebug("Mocking Registry API HTTP request {Method} {Path}", method, path);
    }

    public async Task<RegistryResponse> RegisterTarPayload(TarPayload tarPayload, CancellationToken ct = default)
    {
        var sentAt = DateTime.UtcNow;
        LogRequest("POST", "/api/v1/TAR");

        return new RegistryResponse
        {
            SentAt = sentAt,
            CompletedAt = DateTime.UtcNow,
            Receipt = new TarReceipt
            {
                ReceiptId = Guid.NewGuid(),
                Checksum = "0x7A38BF81F383F69433AD6E900D35B3E2385593F76A7B7AB5D4355B8BA41EE24B",
                Version = 0,
            }
        };
    }

    public async Task<RegistryResponse> UpdateTarPayload(string tarId, TarPayload tarPayload, CancellationToken ct = default)
    {
        var sentAt = DateTime.UtcNow;
        LogRequest("PUT", $"/api/v1/TAR/{tarId}");
        
        return new RegistryResponse
        {
            SentAt = sentAt,
            CompletedAt = DateTime.UtcNow,
            Receipt = new TarReceipt
            {
                ReceiptId = Guid.NewGuid(),
                Checksum = "0x7A38BF81F383F69433AD6E900D35B3E2385593F76A7B7AB5D4355B8BA41EE24B",
                Version = 2,
            }
        };
    }

    public async Task<RegistryResponse> GetTar(Guid receiptId, CancellationToken ct = default)
    {
        var sentAt = DateTime.UtcNow;
        LogRequest("GET", $"/api/v1/TAR/tarId/{receiptId}");

        return new RegistryResponse
        {
            SentAt = sentAt,
            CompletedAt = DateTime.UtcNow,
            Receipt = new TarReceipt
            {
                Id = Random.Shared.Next(0, 100) < 40 ? "urn:tar:xxxx" : null,
                ReceiptId = receiptId,
                Checksum = "0x7A38BF81F383F69433AD6E900D35B3E2385593F76A7B7AB5D4355B8BA41EE24B",
                Version = 2,
            }
        };
    }

    public async Task<RegistryResponse> GetTar(string tarId, CancellationToken ct = default)
    {
        var sentAt = DateTime.UtcNow;
        LogRequest("GET", $"/api/v1/TAR/{tarId}");

        return new RegistryResponse
        {
            SentAt = sentAt,
            CompletedAt = DateTime.UtcNow,
            Receipt = new TarReceipt
            {
                Id = tarId,
                ReceiptId = Guid.NewGuid(),
                Checksum = "0x7A38BF81F383F69433AD6E900D35B3E2385593F76A7B7AB5D4355B8BA41EE24B",
                Version = 2,
            }
        };
    }

    public Task<RegistryResponse> ValidateCallback( /* TODO http context */)
    {
        // verify callback signature based on config secret!

        // parse payload

        throw new NotImplementedException();
    }
}
