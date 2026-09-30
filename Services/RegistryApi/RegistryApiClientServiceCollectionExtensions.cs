using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Compellio.Bcbcti.Options;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.RegistryApi;

public static class RegistryApiClientServiceCollectionExtensions
{
    // TODO config options, HttpClient, JSON serialisation config, etc.

    public static IServiceCollection AddRegistryApi(this IServiceCollection services, IConfiguration configuration)
    {
        // TODO Question: configurable by AddRegistryApi() caller?
        // see BCBCTI.Services.Serialization.Converters.StixJsonConverter
        var serializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        serializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
        // TODO WARNING!!! ONLY THE INNER STIX BUNDLE NEEDS TO BE SERIALIZED WITH stixSerializerOptions
        //                 THE REMAINING TAR ENVELOPE SHOULD NOT (snake case, etc.)!

        services.AddSingleton<IRegistryApiClient>(sp =>
            new RegistryApiClient(sp.GetRequiredService<IOptions<RegistryApiOptions>>(), serializerOptions));

        return services;
    }
}