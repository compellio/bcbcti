using Org.Webpki.JsonCanonicalizer;

namespace Bcbcti.Services.Serialization.Json.Canonicalization;

public class CanonicalJsonSerializer(IJsonSerializer serializer) : IJsonSerializer
{
    public ValueTask<TPayload?> DeserializeAsync<TPayload>(Stream data, CancellationToken ct = default) =>
        serializer.DeserializeAsync<TPayload>(data, ct);

    public async Task SerializeAsync<TPayload>(Stream data, TPayload payload, CancellationToken ct = default)
    {
        await using var temporaryStream = new MemoryStream();
        
        await serializer.SerializeAsync(temporaryStream, payload, ct);
        
        var rawBuffer = temporaryStream.ToArray();
        var canonicalBuffer = new JsonCanonicalizer(rawBuffer).GetEncodedUTF8();
        
        await data.WriteAsync(canonicalBuffer, ct);
    }
}