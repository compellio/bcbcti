using Amazon.S3;
using Microsoft.Extensions.Options;

namespace Bcbcti.Services.Storage.Providers.S3;

public static class S3ObjectStoreServiceCollectionExtensions
{
    public static IServiceCollection AddS3ObjectStore(this IServiceCollection services,
        Action<S3ObjectStoreOptions> configure)
    {
        services.AddAWSService<IAmazonS3>();
        services.AddOptions<S3ObjectStoreOptions>().Configure(configure).ValidateDataAnnotations().ValidateOnStart();

        services.AddSingleton<IObjectStore>(serviceProvider =>
            new S3ObjectStore(serviceProvider.GetRequiredService<IAmazonS3>(),
                serviceProvider.GetRequiredService<IOptions<S3ObjectStoreOptions>>().Value));

        return services;
    }
}