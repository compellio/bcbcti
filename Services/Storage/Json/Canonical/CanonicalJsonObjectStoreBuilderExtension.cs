using Bcbcti.Services.Serialization.Json;
using Bcbcti.Services.Serialization.Json.Canonicalization;

namespace Bcbcti.Services.Storage.Json.Canonical;

public static class CanonicalJsonObjectStoreBuilderExtension
{
    public static JsonObjectStoreBuilder AddCanonicalJsonObjectStore(this JsonObjectStoreBuilder builder)
    {
        return builder.AddObjectStore(serializerOptions =>
            new CanonicalJsonSerializer(new JsonSystemSerializer(serializerOptions)));
    }

    public static JsonObjectStoreBuilder AddKeyedCanonicalJsonObjectStore(this JsonObjectStoreBuilder builder,
        object? key)
    {
        return builder.AddKeyedObjectStore(key,
            serializerOptions => new CanonicalJsonSerializer(new JsonSystemSerializer(serializerOptions)));
    }
}