// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Services.Serialization.Json;
using Compellio.Bcbcti.Services.Serialization.Json.Canonicalization;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.Storage.Json.Canonical;

public static class CanonicalJsonObjectStoreBuilderExtension
{
    public static IServiceCollection AddCanonicalJsonObjectStore(this IServiceCollection services, object? key)
    {
        services.AddKeyedSingleton<IJsonObjectStore>(key, (serviceProvider, _) =>
            new JsonObjectStore(serviceProvider.GetRequiredService<IStreamObjectStore>(),
                new CanonicalJsonSerializer(new JsonSystemSerializer(serviceProvider
                    .GetRequiredService<IOptions<JsonObjectStoreOptions>>()
                    .Value.Serializer))));

        return services;
    }
}