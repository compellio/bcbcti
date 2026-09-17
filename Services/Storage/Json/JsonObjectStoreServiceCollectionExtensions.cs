using System.Text.Json;
using Compellio.Bcbcti.Services.Serialization.Json;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.Storage.Json;

public class JsonObjectStoreOptions
{
    public JsonSerializerOptions Serializer { get; set; } = new();
}

public static class JsonObjectStoreServiceCollectionExtensions
{
    public static IServiceCollection AddJsonObjectStore(this IServiceCollection services,
        Action<JsonSerializerOptions>? configure = null)
    {
        if (configure is not null)
        {
            services.Configure<JsonObjectStoreOptions>(options => configure(options.Serializer));
        }

        services.AddSingleton<IJsonObjectStore>(serviceProvider =>
            new JsonObjectStore(serviceProvider.GetRequiredService<IStreamObjectStore>(),
                new JsonSystemSerializer(serviceProvider.GetRequiredService<IOptions<JsonObjectStoreOptions>>()
                    .Value.Serializer)));

        return services;
    }
}