using System.Text.Json;
using System.Text.Json.Serialization;
using Compellio.Bcbcti.Models.Stix;

namespace Compellio.Bcbcti.Services.Serialization.Json.Converters;

public class StixJsonConverter : JsonConverter<StixObject>
{
    public override StixObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }

    public override void Write(Utf8JsonWriter writer, StixObject value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}