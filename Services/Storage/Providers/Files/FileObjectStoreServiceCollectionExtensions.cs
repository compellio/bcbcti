using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.Storage.Providers.Files;

public static class FileObjectStoreServiceCollectionExtensions
{
    public static IServiceCollection AddFileObjectStore(this IServiceCollection services,
        Action<FileObjectStoreOptions> configure)
    {
        services.AddOptions<FileObjectStoreOptions>().Configure(configure).ValidateDataAnnotations().ValidateOnStart();

        services.AddSingleton<FileObjectStore>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<FileObjectStoreOptions>>().Value;
            return new FileObjectStore(options);
        });

        return services;
    }
}