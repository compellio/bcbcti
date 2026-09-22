namespace Compellio.Bcbcti.Services.Taxii;

public static class TaxiiServiceCollectionExtensions
{
    public static IServiceCollection AddTaxiiServices(this IServiceCollection services)
    {
        return services.AddScoped<StatusService>().AddScoped<TaxiiServerService>().AddScoped<CollectionsService>();
    }
}