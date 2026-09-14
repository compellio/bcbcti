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

        services.AddSingleton<S3ObjectStore>(serviceProvider =>
        {
            var client = serviceProvider.GetRequiredService<IAmazonS3>();
            var options = serviceProvider.GetRequiredService<IOptions<S3ObjectStoreOptions>>().Value;

            // TODO FIXME dirty: casting to read variable
            var forcePathStyle = client.Config is AmazonS3Config { ForcePathStyle: true };

            return new S3ObjectStore(client, options, forcePathStyle);
        });

        return services;
    }
}