using System.Buffers.Text;
using Compellio.Bcbcti.Models.Stix;
using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Services.Storage.Json;
using Compellio.Bcbcti.Services.Storage.Models;

namespace Compellio.Bcbcti.Repositories;

public class StixObjectRepository([FromKeyedServices("canonical")] IJsonObjectStore store) : Repository(store)
{
    // Use of Span<> based on https://medium.com/@sweetondonie/span-t-vs-array-beginner-friendly-explanation-8f9f00e0c0e7
    private string BuildStixObjectKey(ReadOnlySpan<byte> hashBuffer)
    {
        var hash = Base64Url.EncodeToString(hashBuffer);

        return $"objects/{hash}.json";
    }

    public async Task<GetObjectResponse<StixObjectResource>> GetStixObject(string objectKey, CancellationToken ct = default)
    {
        return await Store.GetObjectAsync<StixObjectResource>(objectKey, ct);
    }

    public Task<GetObjectResponse<StixObjectResource>> GetStixObject(ReadOnlySpan<byte> hashBuffer,
        CancellationToken ct = default) =>
        GetStixObject(BuildStixObjectKey(hashBuffer), ct);

    public async Task<PutObjectResponse> StoreStixObject(StixObject entry, CancellationToken ct = default)
    {
        return await Store.PutContentAddressedObjectAsync(hb => BuildStixObjectKey(hb), entry, Condition.IfNoneExists, ct);
    }
}