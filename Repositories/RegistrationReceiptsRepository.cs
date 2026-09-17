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
        // TODO CREATE If-None-Match: * => throw if already exist
        return await Store.PutObjectAsync(BuildJournalKey(receipt.ReceiptId), receipt, ct);
    }
    
    public async Task<PutObjectResponse> UpdateReceipt(RegistrationReceipt receipt, string etag, CancellationToken ct = default)
    {
        // TODO If-Match: {etag} conditions => throw if being processed
        return await StoreReceipt(receipt, ct);
    }

}