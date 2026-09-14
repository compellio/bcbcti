using Bcbcti.Services.Storage.Results;

namespace Bcbcti.Services.Storage.Json;

public interface IJsonObjectStore
{
    public Uri GetObjectUri(string objectKey);
    
    public Task<GetObjectResult<TPayload>> GetObjectAsync<TPayload>(string objectKey, CancellationToken ct = default);
    
    public Task<PutResult> PutObjectAsync<TPayload>(string objectKey, TPayload input, CancellationToken ct = default);
}