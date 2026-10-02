// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Services.Ingestion.Registry;
using Compellio.Bcbcti.Services.RegistryApi.Models;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Compellio.Bcbcti.Services.RegistryApi;

// TODO
public class RegistryApiClient : IRegistryApiClient
{

    private JsonSerializerOptions _serializerOptions;
    private IOptions<RegistryApiOptions> _registryApiOptions;
    private Uri baseUri;

    public RegistryApiClient(IOptions<RegistryApiOptions> options, JsonSerializerOptions serializerOptions/* + http client */)
    {
        _registryApiOptions = options;
        _serializerOptions = serializerOptions;
        baseUri = new Uri(_registryApiOptions.Value.IssuerDomain);
    }
    
    public async Task<RegistryResponse> RegisterTarPayload(TarPayload tarPayload, CancellationToken ct = default)
    {
        var uri = new Uri(baseUri, "/api/v1/TAR");
        using HttpClient client = new();
        var response = await client.PostAsJsonAsync<TarPayload>(uri, tarPayload, ct);
        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync(ct);
            return new RegistryResponse
            {
                SentAt = DateTime.UtcNow,
                Receipt = JsonSerializer.Deserialize<TarReceipt>(responseData)
            };
        }
        else
        {
            throw new InvalidOperationException(response.StatusCode.ToString());
        }

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
        var uri = new Uri(baseUri, $"/api/v1/TAR/{tarId}");
        using HttpClient client = new();
        var response = await client.PutAsJsonAsync<TarPayload>(uri, tarPayload, ct);
        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync(ct);
            return new RegistryResponse
            {
                SentAt = DateTime.UtcNow,
                Receipt = JsonSerializer.Deserialize<TarReceipt>(responseData)
            };
        }
        else
        {
            throw new InvalidOperationException(response.StatusCode.ToString());
        }

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
        var uri = new Uri(baseUri, $"/api/v1/TAR/tarId/{receiptId.ToString()}");
        using HttpClient client = new();
        var response = await client.GetAsync(uri, ct);
        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync(ct);
            return new RegistryResponse
            {
                SentAt = DateTime.UtcNow,
                Receipt = JsonSerializer.Deserialize<TarReceipt>(responseData)
            };
        }
        else
        {
            throw new InvalidOperationException(response.StatusCode.ToString());
        }

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

    public async Task<RegistryResponse> GetTar(string tarId, CancellationToken ct = default)
    {
        var uri = new Uri(baseUri, $"/api/v1/TAR/{tarId}");
        using HttpClient client = new();
        var response = await client.GetAsync(uri, ct);
        if (response.IsSuccessStatusCode)
        {
            var responseData = await response.Content.ReadAsStringAsync(ct);
            return new RegistryResponse
            {
                SentAt = DateTime.UtcNow,
                Receipt = JsonSerializer.Deserialize<TarReceipt>(responseData)
            };
        }
        else
        {
            throw new InvalidOperationException(response.StatusCode.ToString());
        }
    }

    public Task<RegistryResponse> ValidateCallback(/* TODO http context */)
    {
        // verify callback signature based on config secret!
        
        // parse payload
        
        throw new NotImplementedException();
    }
}