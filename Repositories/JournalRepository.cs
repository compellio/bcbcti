using Bcbcti.Models;
using Bcbcti.Services.Storage.Json;
using Bcbcti.Services.Storage.Results;

namespace Bcbcti.Repositories;

public class JournalRepository(IJsonObjectStore store) : Repository(store)
{

    private string BuildJournalKey(Guid id)
    {
        return $"journal/{id}.json";
    }
    
    public async Task<GetObjectResult<JournalEntry>> GetJournalEntry(Guid id, CancellationToken ct = default)
    {
        return await Store.GetObjectAsync<JournalEntry>(BuildJournalKey(id), ct);
    }
    
    public async Task<PutObjectResponse> PutJournalEntry(JournalEntry entry, CancellationToken ct = default)
    {
        // TODO prevent overrides?
        return await Store.PutObjectAsync(BuildJournalKey(entry.Id), entry, ct);
    }

}