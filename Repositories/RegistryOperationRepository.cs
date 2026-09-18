using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Services.Storage.Json;
using Compellio.Bcbcti.Services.Storage.Models;

namespace Compellio.Bcbcti.Repositories;

public class RegistryOperationRepository(IJsonObjectStore store) : Repository(store)
{
    private const string Prefix = "pending/";

    private string BuildOperationKey(string objectId)
    {
        return $"{Prefix}{objectId}.json";
    }

    public Task<GetObjectResponse<RegistryOperation>> GetRegistryOperation(Guid journalId, string objectId,
        CancellationToken ct = default) =>
        GetRegistryOperation(BuildOperationKey(objectId), ct);

    public async Task<GetObjectResponse<RegistryOperation>> GetRegistryOperation(string key,
        CancellationToken ct = default)
    {
        return await Store.GetObjectAsync<RegistryOperation>(key, ct);
    }

    public async Task<PutObjectResponse> CreateRegistryOperation(RegistryOperation entry, CancellationToken ct = default)
    {
        return await Store.PutObjectAsync(BuildOperationKey(entry.ObjectId), entry,
            Condition.IfNoneExists, ct);
    }

    public async Task<PutObjectResponse> UpdateRegistryOperation(string etag, RegistryOperation entry, CancellationToken ct = default)
    {
        return await Store.PutObjectAsync(BuildOperationKey(entry.ObjectId), entry,
            Condition.IfMatch(etag), ct);
    }

    public ListObjectsResponse ListRegistryOperations(string? startAfter = null)
    {
        return Store.ListObjectsAsync(Prefix, startAfter);
    }

    public Task<PutObjectResponse> DeleteRegistryOperation(Guid journalId, string objectId,
        CancellationToken ct = default) =>
        DeleteRegistryOperation(BuildOperationKey(objectId), ct);

    public async Task<PutObjectResponse> DeleteRegistryOperation(string key, CancellationToken ct = default)
    {
        throw new NotImplementedException();
        // return await Store.DeleteObjectAsync(key, entry, PutCondition.IfNoneExists, ct);
    }
}