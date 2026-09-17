using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Services.Storage.Json;
using Compellio.Bcbcti.Services.Storage.Models;

namespace Compellio.Bcbcti.Repositories;

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
        return await Store.PutObjectAsync(BuildJournalKey(entry.Id), entry, PutCondition.IfNoneExists, ct);
    }

}