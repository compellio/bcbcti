using Compellio.Bcbcti.Services.Storage.Models;

namespace Compellio.Bcbcti.Services.Storage.Providers.AzureBlob;

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
    
    public Task<GetObjectResponse<Stream>> GetObjectAsync(string objectKey, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }

    public Task<GetObjectResponse<Stream>?> FindObjectAsync(string objectKey, CancellationToken ct = default)
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

    public Task<ListObjectResponse> ListObjectsAsync(ListObjectRequest request, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
