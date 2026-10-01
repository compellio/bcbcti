// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json;

namespace Compellio.Bcbcti.Services.Serialization.Json;

public class JsonSystemSerializer(JsonSerializerOptions options) : IJsonSerializer
{

    public ValueTask<TPayload?> DeserializeAsync<TPayload>(Stream data, CancellationToken ct = default)
    {
        return JsonSerializer.DeserializeAsync<TPayload>(data, options, ct);
    }

    public Task SerializeAsync<TPayload>(Stream data, TPayload payload, CancellationToken ct = default)
    {
        return JsonSerializer.SerializeAsync(data, payload, options, ct);
    }
    
}