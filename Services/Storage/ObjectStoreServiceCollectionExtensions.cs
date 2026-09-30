using Compellio.Bcbcti.Services.Storage.Providers.FileSystem;
using Compellio.Bcbcti.Services.Storage.Providers.S3;

namespace Compellio.Bcbcti.Services.Storage;

enum ObjectStoreProviders
{
    S3,
    FileSystem,
    // AzureBlob
}

public static class ObjectStoreServiceCollectionExtensions
{
    public static IServiceCollection AddObjectStore(this IServiceCollection services,
        IConfigurationSection configuration)
    {
        var provider = configuration.GetValue<ObjectStoreProviders>("Provider");
        var options = configuration.GetSection(provider.ToString());

        switch (provider)
        {
            case ObjectStoreProviders.S3:
                services.AddS3ObjectStore(options.Bind);
                services.AddSingleton<IStreamObjectStore>(sp => sp.GetRequiredService<S3ObjectStore>());
                break;

            case ObjectStoreProviders.FileSystem:
                services.AddFileSystemObjectStore(options.Bind);
                services.AddSingleton<IStreamObjectStore>(sp => sp.GetRequiredService<FileSystemObjectStore>());
                break;

            default:
                throw new InvalidOperationException($"Unknown storage provider: {provider.ToString()}");
        }

        return services;
    }
}