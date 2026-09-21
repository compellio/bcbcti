using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Services.Storage.Json;
using Compellio.Bcbcti.Services.Storage.Models;

namespace Compellio.Bcbcti.Repositories;

public class ManifestRepository(IJsonObjectStore store) : Repository(store)
{
    private string BuildOperationKey(DateTime completedAt, Guid receiptId)
    {
        return $"manifest/{completedAt:yyyy}/{completedAt:MM}/{completedAt:dd}/{completedAt:O}--{receiptId}.json";
    }

    public Task<GetObjectResponse<RegistryOperation>> GetManifestEntry(DateTime completedAt, Guid receiptId,
        CancellationToken ct = default) =>
        GetManifestEntry(BuildOperationKey(completedAt, receiptId), ct);

    public Task<GetObjectResponse<RegistryOperation>> GetManifestEntry(string key, CancellationToken ct = default)
    {
        return Store.GetObjectAsync<RegistryOperation>(key, ct);
    }

    public async Task<PutObjectResponse> CreateManifestEntry(ManifestEntry entry, CancellationToken ct = default)
    {
        return await Store.PutObjectAsync(BuildOperationKey(entry.CompletedAt, entry.ReceiptId), entry,
            Condition.IfNoneExists, ct);
    }

    public ListObjectsResponse ListManifestEntries(DateTime? startAfter = null)
    {
        throw new NotImplementedException();
    }
}