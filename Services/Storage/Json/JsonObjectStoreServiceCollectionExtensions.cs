using System.Text.Json;
using Bcbcti.Services.Serialization.Json;
using Microsoft.Extensions.Options;

namespace Bcbcti.Services.Storage.Json;

public class JsonObjectStoreOptions
{
    public JsonSerializerOptions Serializer { get; set; } = new();
}

public class JsonObjectStoreBuilder(IServiceCollection services)
{
    public IServiceCollection Services { get; } = services;

    public JsonObjectStoreBuilder AddObjectStore(Func<JsonSerializerOptions, IJsonSerializer> serializerFactory)
    {
        Services.AddSingleton<IJsonObjectStore>(serviceProvider =>
            new JsonObjectStore(serviceProvider.GetRequiredService<IStreamObjectStore>(),
                serializerFactory(serviceProvider.GetRequiredService<IOptions<JsonObjectStoreOptions>>()
                    .Value.Serializer)));

        return this;
    }

    public JsonObjectStoreBuilder AddKeyedObjectStore(object? key,
        Func<JsonSerializerOptions, IJsonSerializer> serializerFactory)
    {
        Services.AddKeyedSingleton<IJsonObjectStore>(key,
            (serviceProvider, _) => new JsonObjectStore(serviceProvider.GetRequiredService<IStreamObjectStore>(),
                serializerFactory(serviceProvider.GetRequiredService<IOptions<JsonObjectStoreOptions>>()
                    .Value.Serializer)));

        return this;
    }
}

public static class JsonObjectStoreServiceCollectionExtensions
{
    public static JsonObjectStoreBuilder AddJsonObjectStore(this IServiceCollection services)
    {
        var builder = new JsonObjectStoreBuilder(services);

        return builder.AddObjectStore(serializerOptions => new JsonSystemSerializer(serializerOptions));
    }

    public static JsonObjectStoreBuilder AddKeyedJsonObjectStore(this JsonObjectStoreBuilder builder, object? key)
    {
        return builder.AddKeyedObjectStore(key, serializerOptions => new JsonSystemSerializer(serializerOptions));
    }

    public static JsonObjectStoreBuilder AddJsonOptions(this JsonObjectStoreBuilder builder,
        Action<JsonSerializerOptions> configure)
    {
        builder.Services.Configure<JsonObjectStoreOptions>(options => configure(options.Serializer));
        return builder;
    }
}