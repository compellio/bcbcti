using Bcbcti.Services.Serialization.Json;
using Bcbcti.Services.Storage.Results;

namespace Bcbcti.Services.Storage.Json;

public class JsonObjectStore : IJsonObjectStore
{

    private readonly IStreamObjectStore _store;
    private readonly IJsonSerializer _serializer;

    public JsonObjectStore(IStreamObjectStore store, IJsonSerializer serializer)
    {
        _store = store;
        _serializer = serializer;
    }

    public Uri GetObjectUri(string objectKey) => _store.GetObjectUri(objectKey);

    public async Task<GetObjectResult<TPayload>> GetObjectAsync<TPayload>(string objectKey, CancellationToken ct = default)
    {
        var response = await _store.GetObjectAsync(objectKey, ct);
        
        await using var body = response.Body;
        
        var payload = await _serializer.DeserializeAsync<TPayload>(body, ct);

        if (payload is null)
        {
            throw new InvalidDataException($"Object '{objectKey}' deserialization error.");
        }

        return new GetObjectResult<TPayload>
        {
            Body = payload,
            Metadata = response.Metadata
        };
    }

    public async Task<PutResult> PutObjectAsync<TPayload>(string objectKey, TPayload input, CancellationToken ct = default)
    {
        using var payload = new MemoryStream();
        await _serializer.SerializeAsync(payload, input, ct);
        payload.Position = 0;
        
        return await _store.PutObjectAsync(objectKey, payload, ct);
    }
    
}