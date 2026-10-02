// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.RegistryApi;

public static class RegistryApiClientServiceCollectionExtensions
{
    // TODO config HttpClient, etc.

    public static IServiceCollection AddRegistryApi(this IServiceCollection services, IConfiguration configuration, Action<JsonSerializerOptions>? configure = null)
    {
        services.AddOptions<RegistryApiOptions>().Configure(configuration.Bind).ValidateDataAnnotations().ValidateOnStart();

        var serializerOptions = new JsonSerializerOptions();

        configure?.Invoke(serializerOptions);

        services.AddSingleton<IRegistryApiClient>(sp =>
            new RegistryApiClient(sp.GetRequiredService<IOptions<RegistryApiOptions>>(), serializerOptions));

        return services;
    }
}
