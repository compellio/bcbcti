using System.Text.Json;
using Bcbcti.Services.Storage.Adapters;
using Bcbcti.Services.Storage.Providers.S3;

namespace Bcbcti.Services.Storage;

enum ObjectStoreProviders
{
    S3,
    // AzureBlob
}

public static class ObjectStoreServiceCollectionExtensions
{
    public static IServiceCollection AddObjectStore(this IServiceCollection services,
        IConfigurationSection configuration)
    {
        var provider = configuration.GetValue<ObjectStoreProviders>("Provider");
        var options = configuration.GetSection("ProviderOptions");

        return provider switch
        {
            ObjectStoreProviders.S3 => services.AddS3ObjectStore(options.Bind),
            _ => throw new InvalidOperationException($"Unsupported storage kind: {provider.ToString()}")
        };
    }

    public static IServiceCollection AddJsonObjectStore(this IServiceCollection services, Action<JsonSerializerOptions> configure)
    {
        var options = new JsonSerializerOptions();
        configure(options);
        
        services.AddSingleton<JsonObjectStoreAdapter>(serviceProvider =>
            new JsonObjectStoreAdapter(serviceProvider.GetRequiredService<IObjectStore>(), options));

        return services;
    }
}