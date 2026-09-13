using Bcbcti.Services.Storage.Responses;

namespace Bcbcti.Services.Storage.Providers.AzureBlob;

public class AzureBlobObjectStore : IObjectStore
{
    public Task<GetResponse> GetObjectAsync(string objectKey, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}