using System.Text.Json;

namespace Bcbcti.Services.Serialization.Json;

public class JsonSystemSerializer(JsonSerializerOptions options) : IJsonSerializer
{

    public ValueTask<TPayload?> DeserializeAsync<TPayload>(Stream data, CancellationToken ct = default)
    {
        return JsonSerializer.DeserializeAsync<TPayload>(data, options, ct);
    }

    public Task SerializeAsync<TPayload>(Stream data, TPayload payload, CancellationToken ct = default)
    {
        return JsonSerializer.SerializeAsync(data, payload, options, ct);
    }
    
}