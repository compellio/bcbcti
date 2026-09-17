using Compellio.Bcbcti.Services.Storage.Models;

namespace Compellio.Bcbcti.Services.Storage.Json;

public interface IJsonObjectStore
{
    public Task<GetObjectResponse<TPayload>> GetObjectAsync<TPayload>(string objectKey, CancellationToken ct = default);

    public Task<PutObjectResponse> PutObjectAsync<TPayload>(string objectKey, TPayload input,
        CancellationToken ct = default);

    public Task<PutObjectResponse> PutObjectAsync<TPayload>(string objectKey, TPayload input, PutCondition condition,
        CancellationToken ct = default);

    /// <summary>
    /// Similar to PutObjectAsync but for storing objects keys containing the stored content hash.
    /// TODO REVIEW (interface segregration) method should be moved to a separate/sibling IJsonObjectStore-ish class
    /// </summary>
    public Task<PutObjectResponse> PutContentAddressedObjectAsync<TPayload>(Func<byte[], string> keyFactory,
        TPayload input, CancellationToken ct = default);

    public Task<PutObjectResponse> PutContentAddressedObjectAsync<TPayload>(Func<byte[], string> keyFactory,
        TPayload input, PutCondition condition, CancellationToken ct = default);
}