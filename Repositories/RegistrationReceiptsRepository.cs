using Bcbcti.Models.Documents;
using Bcbcti.Services.Storage.Json;
using Bcbcti.Services.Storage.Models;

namespace Bcbcti.Repositories;

public class RegistrationReceiptsRepository(IJsonObjectStore store) : Repository(store)
{

    private string BuildJournalKey(Guid id)
    {
        return $"receipts/{id}.json";
    }
    
    public async Task<GetObjectResponse<JournalEntry>> GetReceipt(Guid id, CancellationToken ct = default)
    {
        return await Store.GetObjectAsync<JournalEntry>(BuildJournalKey(id), ct);
    }
    
    public async Task<PutObjectResponse> StoreReceipt(RegistrationReceipt receipt, CancellationToken ct = default)
    {
        return await Store.PutObjectAsync(BuildJournalKey(receipt.ReceiptId), receipt, PutCondition.IfNoneExists, ct);
    }
    
    public async Task<PutObjectResponse> UpdateReceipt(RegistrationReceipt receipt, string etag, CancellationToken ct = default)
    {
        return await Store.PutObjectAsync(BuildJournalKey(receipt.ReceiptId), receipt, PutCondition.IfMatch(etag), ct);
    }

}