using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Services.Storage.Json;
using Compellio.Bcbcti.Services.Storage.Models;

namespace Compellio.Bcbcti.Repositories;

public class ManifestRepository(IJsonObjectStore store) : Repository(store)
{
    private const string Prefix = "manifest/";

    private string BuildManifestEntryPartition(DateTime completedAt)
    {
        var utcDate = completedAt.ToUniversalTime();
        return $"{utcDate:yyyy}/{utcDate:MM}/{utcDate:dd}/{utcDate:O}";
    }

    private string BuildManifestEntryKey(DateTime completedAt, Guid receiptId)
    {
        var partition = BuildManifestEntryPartition(completedAt);
        return $"{Prefix}{partition}--{receiptId}.json";
    }

    public Task<GetObjectResponse<ManifestEntry>> GetManifestEntry(DateTime completedAt, Guid receiptId,
        CancellationToken ct = default) =>
        GetManifestEntry(BuildManifestEntryKey(completedAt, receiptId), ct);

    public Task<GetObjectResponse<ManifestEntry>> GetManifestEntry(string key, CancellationToken ct = default)
    {
        return Store.GetObjectAsync<ManifestEntry>(key, ct);
    }

    public async Task<PutObjectResponse> PutManifestEntry(ManifestEntry entry, CancellationToken ct = default)
    {
        return await Store.PutObjectAsync(BuildManifestEntryKey(entry.CompletedAt, entry.ReceiptId), entry, ct);
    }

    public ListObjectsResponse ListManifestEntries(DateTime? addedAfter = null)
    {
        var startAfter = addedAfter.HasValue ? Prefix + BuildManifestEntryPartition(addedAfter.Value) : null;

        return Store.ListObjectsAsync(Prefix, startAfter);
    }
}