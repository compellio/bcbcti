using System.Text.Json;
using Bcbcti.Services.Storage.Responses;

namespace Bcbcti.Services.Storage.Adapters;

public class JsonObjectStoreAdapter
{

    private readonly IObjectStore _store;
    private readonly JsonSerializerOptions _options;

    public JsonObjectStoreAdapter(IObjectStore store, JsonSerializerOptions options)
    {
        _store = store;
        _options = options;
    }

    public async Task<GetResponse<TPayload>> GetObjectAsync<TPayload>(string objectKey, CancellationToken ct = default)
    {
        var response = await _store.GetObjectAsync(objectKey, ct);

        await using var body = response.Body;
        
        // TODO JCS canonicalization!!
        
        var payload = await JsonSerializer.DeserializeAsync<TPayload>(body, _options, ct);

        if (payload is null)
        {
            throw new InvalidDataException($"Object '{objectKey}' deserialization error.");
        }

        return new GetResponse<TPayload>
        {
            Body = payload,
            Metadata = response.Metadata
        };
    }

    public async Task<PutResponse> PutObjectAsync<TPayload>(string objectKey, TPayload input, CancellationToken ct = default)
    {
        using var payload = new MemoryStream();
        await JsonSerializer.SerializeAsync(payload, input, _options, ct);
        payload.Position = 0;
        
        return await _store.PutObjectAsync(objectKey, payload, ct);
    }
    
}