using Compellio.Bcbcti.Options;

namespace Compellio.Bcbcti.Services.Taxii;

public static class TaxiiServiceCollectionExtensions
{
    public static IServiceCollection AddTaxiiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<TaxiiOptions>()
            .Bind(configuration)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddScoped<StatusService>().AddScoped<TaxiiServerService>().AddScoped<CollectionsService>()
            .AddScoped<ManifestService>();

        return services;
    }
}