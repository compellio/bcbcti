using Bcbcti.Services.Storage.Results;
using Org.Webpki.JsonCanonicalizer;

namespace Bcbcti.Services.Storage.Json.Canonicalization;

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

    public async Task<PutResult> PutObjectAsync(string objectKey, Stream input, CancellationToken ct = default)
    {
        byte[] raw;
        
        if (input is MemoryStream stream)
        {
            raw = stream.ToArray();
        }
        else
        {
            using var temporaryStream = new MemoryStream();
            await input.CopyToAsync(temporaryStream, ct);
            raw = temporaryStream.ToArray();
        }
        
        var canonical = new JsonCanonicalizer(raw).GetEncodedUTF8();
        
        await using var canonicalStream = new MemoryStream(canonical, writable: false);
        
        return await _store.PutObjectAsync(objectKey, canonicalStream, ct);
    }
}