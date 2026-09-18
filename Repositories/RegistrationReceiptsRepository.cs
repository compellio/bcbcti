using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Services.Storage.Json;
using Compellio.Bcbcti.Services.Storage.Models;

namespace Compellio.Bcbcti.Repositories;

public class RegistrationReceiptsRepository(IJsonObjectStore store) : Repository(store)
{

    private string BuildJournalKey(Guid id)
    {
        return $"receipts/{id}.json";
    }
    
    public async Task<GetObjectResponse<RegistrationReceipt>> GetReceipt(Guid id, CancellationToken ct = default)
    {
        return await Store.GetObjectAsync<RegistrationReceipt>(BuildJournalKey(id), ct);
    }
    
    public async Task<PutObjectResponse> StoreReceipt(RegistrationReceipt receipt, CancellationToken ct = default)
    {
        return await Store.PutObjectAsync(BuildJournalKey(receipt.ReceiptId), receipt, PutCondition.IfNoneExists, ct);
    }
    
    public async Task<PutObjectResponse> UpdateReceipt(string etag, RegistrationReceipt receipt, CancellationToken ct = default)
    {
        return await Store.PutObjectAsync(BuildJournalKey(receipt.ReceiptId), receipt, PutCondition.IfMatch(etag), ct);
    }

}