using Bcbcti.Models.Documents;
using Bcbcti.Services.Storage.Json;
using Bcbcti.Services.Storage.Models;

namespace Bcbcti.Repositories;

public class JournalRepository(IJsonObjectStore store) : Repository(store)
{

    private string BuildJournalKey(Guid id)
    {
        return $"journal/{id}.json";
    }
    
    public async Task<GetObjectResponse<JournalEntry>> GetJournalEntry(Guid id, CancellationToken ct = default)
    {
        return await Store.GetObjectAsync<JournalEntry>(BuildJournalKey(id), ct);
    }
    
    public async Task<PutObjectResponse> PutJournalEntry(JournalEntry entry, CancellationToken ct = default)
    {
        // TODO CREATE = If-None-Match: * => throw if already exist
        return await Store.PutObjectAsync(BuildJournalKey(entry.Id), entry, ct);
    }

}