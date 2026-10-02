// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.RegistryApi;

public static class RegistryApiClientServiceCollectionExtensions
{
    // TODO config HttpClient, etc.

    public static IServiceCollection AddRegistryApi(this IServiceCollection services, IConfiguration configuration,
        Action<JsonSerializerOptions>? configure = null)
    {
        services.AddOptions<RegistryApiOptions>().Configure(configuration.Bind).ValidateDataAnnotations().ValidateOnStart();

        var serializerOptions = new JsonSerializerOptions();

        configure?.Invoke(serializerOptions);

        services.AddHttpClient<IRegistryApiClient, RegistryApiClient>((client, sp) =>
        {
            var options = sp.GetRequiredService<IOptions<RegistryApiOptions>>().Value;

            client.BaseAddress = new Uri(options.ServiceUrl);
            client.DefaultRequestHeaders.Add("X-Session-Key", options.ApiKey);

            if (options.Network is not null)
            {
                client.DefaultRequestHeaders.Add("X-Blockchain", options.Network);
            }

            if (options.IssuerDomain is not null)
            {
                client.DefaultRequestHeaders.Add("X-Issuer-Domain", options.IssuerDomain);
            }

            return new RegistryApiClient(client, serializerOptions);
        });

        return services;
    }
}
