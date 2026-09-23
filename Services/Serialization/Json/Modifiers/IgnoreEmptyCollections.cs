using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace Compellio.Bcbcti.Services.Serialization.Json.Modifiers;

// TODO REVIEW
// based on
//   https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/custom-contracts#example-ignore-properties-with-a-specific-type
//   https://stackoverflow.com/questions/18471864/how-to-make-json-net-skip-serialization-of-empty-collections
public static class IgnoreEmptyCollections
{
    public static void ModifyTypeInfo(JsonTypeInfo typeInfo)
    {
        foreach (JsonPropertyInfo propertyInfo in typeInfo.Properties)
        {
            if (propertyInfo.PropertyType != typeof(string) &&
                propertyInfo.PropertyType.IsAssignableTo(typeof(IEnumerable)))
            {
                propertyInfo.ShouldSerialize = (prop, value) => value is not ICollection { Count: 0 };
            }
        }
    }
}