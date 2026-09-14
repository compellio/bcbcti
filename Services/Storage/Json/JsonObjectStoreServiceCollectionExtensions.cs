using System.Text.Json;

namespace Bcbcti.Services.Storage.Json;

public static class JsonObjectStoreServiceCollectionExtensions
{
    public static IServiceCollection AddJsonObjectStore(this IServiceCollection services,
        Action<JsonSerializerOptions> configure)
    {
        var options = new JsonSerializerOptions();
        configure(options);

        // todo wrap IStreamObjectStore in JcsObjectStore (canonicalization decorator)

        services.AddSingleton<IJsonObjectStore>(serviceProvider =>
            new JsonObjectStore(serviceProvider.GetRequiredService<IStreamObjectStore>(), options));

        return services;
    }
}