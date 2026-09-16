using Bcbcti.Services.Storage.Results;

namespace Bcbcti.Services.Storage.Json;

public interface IJsonObjectStore
{
    public Task<GetObjectResult<TPayload>> GetObjectAsync<TPayload>(string objectKey, CancellationToken ct = default);

    public Task<PutObjectResponse> PutObjectAsync<TPayload>(string objectKey, TPayload input,
        CancellationToken ct = default);

    /// <summary>
    /// Similar to PutObjectAsync but for storing objects keys containing the stored content hash.
    /// </summary>
    // TODO REVIEW (interface segregration) method should be moved to a separate/sibling IJsonObjectStore-ish class
    public Task<PutObjectResponse> PutContentAddressedObjectAsync<TPayload>(
        Func<byte[], string> keyFactory,TPayload input,
        CancellationToken ct = default);
}