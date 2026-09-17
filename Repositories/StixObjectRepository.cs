using System.Buffers.Text;
using Bcbcti.Models.Stix;
using Bcbcti.Services.Storage.Json;
using Bcbcti.Services.Storage.Models;

namespace Bcbcti.Repositories;

public class StixObjectRepository([FromKeyedServices("canonical")] IJsonObjectStore store) : Repository(store)
{
    // Use of Span<> based on https://medium.com/@sweetondonie/span-t-vs-array-beginner-friendly-explanation-8f9f00e0c0e7
    private string BuildStixObjectKey(ReadOnlySpan<byte> hashBuffer)
    {
        var hash = Base64Url.EncodeToString(hashBuffer);

        return $"objects/{hash}.json";
    }

    public async Task<GetObjectResponse<StixObject>> GetStixObject(string objectKey, CancellationToken ct = default)
    {
        return await Store.GetObjectAsync<StixObject>(objectKey, ct);
    }

    public Task<GetObjectResponse<StixObject>> GetStixObject(ReadOnlySpan<byte> hashBuffer,
        CancellationToken ct = default) =>
        GetStixObject(BuildStixObjectKey(hashBuffer), ct);

    public async Task<PutObjectResponse> StoreStixObject(StixObject entry, CancellationToken ct = default)
    {
        // TODO CREATE = If-None-Match: * => throw if already exist
        //      (REVIEW: or not in this particular case? if same object already exists means it's already stored, carry on?)
        return await Store.PutContentAddressedObjectAsync(hb => BuildStixObjectKey(hb), entry, ct);
    }
}