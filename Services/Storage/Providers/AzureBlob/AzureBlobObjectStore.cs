using Bcbcti.Services.Storage.Responses;

namespace Bcbcti.Services.Storage.Providers.AzureBlob;

public class AzureBlobObjectStore : IObjectStore
{
    public Uri GetObjectUri(string objectKey)
    {
        throw new NotImplementedException();
    }
    
    public Task<GetResponse> GetObjectAsync(string objectKey, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<PutResponse> PutObjectAsync(string objectKey, Stream input, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
