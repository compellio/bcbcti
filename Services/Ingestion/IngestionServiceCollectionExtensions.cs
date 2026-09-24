using Compellio.Bcbcti.Options;

namespace Compellio.Bcbcti.Services.Ingestion;

public static class IngestionServiceCollectionExtensions
{
    public static IServiceCollection AddIngestion(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<IngestionOptions>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddSingleton<StixIngestionService>();
        services.AddSingleton<StixReconciliationService>();
        services.AddHostedService<ReconciliationHostedService>();

        return services;
    }
}