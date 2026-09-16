using Bcbcti.Services.Serialization.Json;
using Bcbcti.Services.Storage.Models;

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

    private async Task<MemoryStream> Serialize<TPayload>(TPayload input, CancellationToken ct = default)
    {
        var payload = new MemoryStream();
        await _serializer.SerializeAsync(payload, input, ct);
        payload.Position = 0;

        return payload;
    }

    public Uri GetObjectUri(string objectKey) => _store.GetObjectUri(objectKey);

    public async Task<GetObjectResponse<TPayload>> GetObjectAsync<TPayload>(string objectKey,
        CancellationToken ct = default)
    {
        var response = await _store.GetObjectAsync(objectKey, ct);

        await using var body = response.Body;

        var payload = await _serializer.DeserializeAsync<TPayload>(body, ct);

        if (payload is null)
        {
            throw new InvalidDataException($"Object '{objectKey}' deserialization error.");
        }

        return new GetObjectResponse<TPayload> { Body = payload, Metadata = response.Metadata };
    }

    public async Task<PutObjectResponse> PutObjectAsync<TPayload>(string objectKey, TPayload input,
        CancellationToken ct = default)
    {
        using var payload = await Serialize(input, ct);

        var request = new PutObjectRequest
        {
            ObjectKey = objectKey,
            InputStream = payload,
        };

        return await _store.PutObjectAsync(request, ct);
    }

    public async Task<PutObjectResponse> PutContentAddressedObjectAsync<TPayload>(Func<byte[], string> keyFactory,
        TPayload input, CancellationToken ct = default)
    {
        using var payload = await Serialize(input, ct);
        var hashBuffer = await _store.ComputeSha256Hash(payload, ct);

        var request = new PutObjectRequest
        {
            ObjectKey = keyFactory(hashBuffer),
            InputStream = payload,
            ChecksumSHA256 = hashBuffer
        };

        return await _store.PutObjectAsync(request, ct);
    }
}