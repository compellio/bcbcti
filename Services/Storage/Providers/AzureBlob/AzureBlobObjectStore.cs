using Bcbcti.Services.Storage.Results;

namespace Bcbcti.Services.Storage.Providers.AzureBlob;

public class AzureBlobObjectStore : IStreamObjectStore
{
    public Uri GetObjectUri(string objectKey)
    {
        throw new NotImplementedException();
    }
    
    public Task<GetObjectResult<Stream>> GetObjectAsync(string objectKey, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<PutResult> PutObjectAsync(string objectKey, Stream input, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
