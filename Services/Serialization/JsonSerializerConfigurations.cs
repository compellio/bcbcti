using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using Compellio.Bcbcti.Services.Serialization.Json.Converters;
using Compellio.Bcbcti.Services.Serialization.Json.Modifiers;

namespace Compellio.Bcbcti.Services.Serialization;

public static class JsonSerializerConfigurations
{

    private static void Base(JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));
        options.Converters.Add(new StixTimestampConverter());
    }

    public static void Taxii(JsonSerializerOptions options)
    {
        Base(options);
        
        options.WriteIndented = true;

        options.TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers =
            {
                IgnoreEmptyCollections.ModifyTypeInfo
            }
        };
        
        options.Converters.Add(new TaxiiTimestampConverter());
    }

    public static void Storage(JsonSerializerOptions options)
    {
        Base(options);
    }
    
    // TODO registry options -> or use Storage as well?
    
}