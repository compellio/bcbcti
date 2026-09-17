namespace Compellio.Bcbcti.Services.RegistryApi;

public static class RegistryApiClientServiceCollectionExtensions
{
    
    // TODO config options, HttpClient, JSON serialisation config, etc.

    public static IServiceCollection AddRegistryApi(this IServiceCollection services)
    {
        services.AddSingleton<IRegistryApiClient>(new RegistryApiClient());

        return services;
    }
    
}