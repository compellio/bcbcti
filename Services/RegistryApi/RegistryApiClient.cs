// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json;
using Compellio.Bcbcti.Services.Ingestion.Registry;
using Compellio.Bcbcti.Services.RegistryApi.Models;

namespace Compellio.Bcbcti.Services.RegistryApi;

public class RegistryApiClient : IRegistryApiClient
{
    private readonly JsonSerializerOptions _serializerOptions;

    private readonly HttpClient _client;

    public RegistryApiClient(HttpClient client, JsonSerializerOptions serializerOptions)
    {
        _client = client;
        _serializerOptions = serializerOptions;
    }

    private async Task<TarReceipt> ReadTarReceipt(HttpResponseMessage response, CancellationToken ct)
    {
        var tarReceipt = await response.Content.ReadFromJsonAsync<TarReceipt>(_serializerOptions, ct);

        if (tarReceipt is null)
        {
            throw new NotImplementedException(); // TODO
        }

        return tarReceipt;
    }

    public async Task<RegistryResponse> RegisterTarPayload(TarPayload tarPayload, CancellationToken ct = default)
    {
        var sentAt = DateTime.UtcNow;
        var response = await _client.PostAsJsonAsync("/api/v1/TAR", tarPayload, _serializerOptions, ct);

        response.EnsureSuccessStatusCode();

        var tarReceipt = await ReadTarReceipt(response, ct);

        return new RegistryResponse
        {
            SentAt = sentAt,
            CompletedAt = DateTime.UtcNow,
            Receipt = tarReceipt
        };
    }

    public async Task<RegistryResponse> UpdateTarPayload(string tarId, TarPayload tarPayload, CancellationToken ct = default)
    {
        var sentAt = DateTime.UtcNow;
        var response = await _client.PutAsJsonAsync($"/api/v1/TAR/{tarId}", tarPayload, _serializerOptions, ct);

        response.EnsureSuccessStatusCode();

        var tarReceipt = await ReadTarReceipt(response, ct);

        return new RegistryResponse
        {
            SentAt = sentAt,
            CompletedAt = DateTime.UtcNow,
            Receipt = tarReceipt
        };
    }

    public async Task<RegistryResponse> GetTar(Guid receiptId, CancellationToken ct = default)
    {
        var sentAt = DateTime.UtcNow;
        var response = await _client.GetAsync($"/api/v1/TAR/tarId/{receiptId.ToString()}", ct);

        response.EnsureSuccessStatusCode();

        var tarReceipt = await ReadTarReceipt(response, ct);

        return new RegistryResponse
        {
            SentAt = sentAt,
            CompletedAt = DateTime.UtcNow,
            Receipt = tarReceipt
        };
    }

    public async Task<RegistryResponse> GetTar(string tarId, CancellationToken ct = default)
    {
        var sentAt = DateTime.UtcNow;
        var response = await _client.GetAsync($"/api/v1/TAR/{tarId}", ct);

        response.EnsureSuccessStatusCode();

        var tarReceipt = await ReadTarReceipt(response, ct);

        return new RegistryResponse
        {
            SentAt = sentAt,
            CompletedAt = DateTime.UtcNow,
            Receipt = tarReceipt
        };
    }

    public Task<RegistryResponse> ValidateCallback( /* TODO http context - perhaps move out of RegistryApiClient? */)
    {
        // verify callback signature based on config secret!

        // parse payload

        throw new NotImplementedException();
    }
}
