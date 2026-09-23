using System.Text.Json;
using System.Text.Json.Serialization;
using Compellio.Bcbcti.Services.Serialization.Primitives;

namespace Compellio.Bcbcti.Services.Serialization.Json.Converters;

// see https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/converters-how-to
public sealed class StixTimestampConverter : JsonConverter<StixTimestamp>
{
    public override StixTimestamp Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new StixTimestamp(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, StixTimestamp value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Raw);
    }
}