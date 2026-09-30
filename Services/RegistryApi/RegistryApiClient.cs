// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Services.Ingestion.Registry;
using Compellio.Bcbcti.Services.RegistryApi.Models;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.RegistryApi;

// TODO
public class RegistryApiClient : IRegistryApiClient
{

    private JsonSerializerOptions _serializerOptions;

    public RegistryApiClient(IOptions<RegistryApiOptions> options, JsonSerializerOptions serializerOptions/* + http client */)
    {
        _serializerOptions = serializerOptions;
    }
    
    public async Task<RegistryResponse> RegisterTarPayload(TarPayload tarPayload, CancellationToken ct = default)
    {
        var data = JsonSerializer.Serialize(tarPayload, _serializerOptions);

        Console.WriteLine($"TODO [POST /api/v1/TAR]\n{data}");
        // dummy response (!careful: checksum in hex, not base64)

        return new RegistryResponse
        {
            SentAt = DateTime.UtcNow,
            Receipt = new TarReceipt
            {
                ReceiptId = Guid.NewGuid(),
                Checksum = "0x7A38BF81F383F69433AD6E900D35B3E2385593F76A7B7AB5D4355B8BA41EE24B",
                Version = 0,
                Data = JsonDocument.Parse(data),
            }
        };
    }

    public async Task<RegistryResponse> UpdateTarPayload(string tarId, TarPayload tarPayload, CancellationToken ct = default)
    {        
        var data = JsonSerializer.Serialize(tarPayload, _serializerOptions);
        
        Console.WriteLine($"TODO [PUT /api/v1/TAR/{tarId}]\n{data}");
        
        return new RegistryResponse
        {
            SentAt = DateTime.UtcNow,
            Receipt = new TarReceipt
            {
                ReceiptId = Guid.NewGuid(),
                Checksum = "0x7A38BF81F383F69433AD6E900D35B3E2385593F76A7B7AB5D4355B8BA41EE24B",
                Version = 2,
                Data = JsonDocument.Parse(data),
            }
        };
    }

    public async Task<RegistryResponse> GetTar(Guid receiptId, CancellationToken ct = default)
    {
        return new RegistryResponse
        {
            SentAt = DateTime.UtcNow,
            Receipt = new TarReceipt
            {
                Id = Random.Shared.Next(0, 100) < 40 ? "urn:tar:xxxx" : null,
                ReceiptId = receiptId,
                Checksum = "0x7A38BF81F383F69433AD6E900D35B3E2385593F76A7B7AB5D4355B8BA41EE24B",
                Version = 2,
                Data = JsonDocument.Parse("{}")
            }
        };
    }

    public Task<RegistryResponse> GetTar(string tarId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<RegistryResponse> ValidateCallback(/* TODO http context */)
    {
        // verify callback signature based on config secret!
        
        // parse payload
        
        throw new NotImplementedException();
    }
}