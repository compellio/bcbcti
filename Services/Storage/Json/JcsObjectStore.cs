using Bcbcti.Services.Storage.Results;

namespace Bcbcti.Services.Storage.Json;

public class JcsObjectStore : IStreamObjectStore
{
    private readonly IStreamObjectStore _store;

    public JcsObjectStore(IStreamObjectStore store)
    {
        _store = store;
    }

    public Uri GetObjectUri(string objectKey) => _store.GetObjectUri(objectKey);

    public Task<GetObjectResult<Stream>> GetObjectAsync(string objectKey, CancellationToken ct = default) =>
        _store.GetObjectAsync(objectKey, ct);

    public Task<PutResult> PutObjectAsync(string objectKey, Stream input, CancellationToken ct = default)
    {
        // TODO apply json canonicalisation to the input stream
        return _store.PutObjectAsync(objectKey, input, ct);
    }
}