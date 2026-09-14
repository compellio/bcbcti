using System.Text.Json;
using Bcbcti.Services.Storage.Json.Canonicalization;

namespace Bcbcti.Services.Storage.Json;

public static class JsonObjectStoreServiceCollectionExtensions
{
    public static IServiceCollection AddJsonObjectStore(this IServiceCollection services,
        Action<JsonSerializerOptions> configure)
    {
        var options = new JsonSerializerOptions();
        configure(options);

        services.AddSingleton<IJsonObjectStore>(serviceProvider =>
            new JsonObjectStore(new JcsObjectStore(serviceProvider.GetRequiredService<IStreamObjectStore>()), options));

        return services;
    }
}