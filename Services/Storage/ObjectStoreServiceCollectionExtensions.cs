using Compellio.Bcbcti.Services.Storage.Providers.S3;

namespace Compellio.Bcbcti.Services.Storage;

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

        switch (provider)
        {
            case ObjectStoreProviders.S3:
                services.AddS3ObjectStore(options.Bind);
                services.AddSingleton<IStreamObjectStore>(sp => sp.GetRequiredService<S3ObjectStore>());
                break;

            default:
                throw new InvalidOperationException($"Unsupported storage kind: {provider.ToString()}");
        }

        return services;
    }
}