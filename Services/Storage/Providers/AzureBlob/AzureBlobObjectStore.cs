using Bcbcti.Services.Storage.Results;

namespace Bcbcti.Services.Storage.Providers.AzureBlob;

public class AzureBlobObjectStore : IStreamObjectStore
{
    public Uri GetObjectUri(string objectKey)
    {
        throw new NotImplementedException();
    }
    
    public Task<byte[]> ComputeSha256Hash(Stream input, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
    
    public Task<GetObjectResult<Stream>> GetObjectAsync(string objectKey, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<PutObjectResponse> PutObjectAsync(string objectKey, Stream input, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
    
    public Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
