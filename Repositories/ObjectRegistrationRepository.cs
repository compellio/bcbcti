using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Services.Storage.Json;
using Compellio.Bcbcti.Services.Storage.Models;

namespace Compellio.Bcbcti.Repositories;

public class ObjectRegistrationRepository(IJsonObjectStore store) : Repository(store)
{

    private string BuildRegistrationStateKey(string id)
    {
        return $"registrations/{id}.json";
    }
    
    public async Task<GetObjectResponse<ObjectRegistration>> GetReceipt(string id, CancellationToken ct = default)
    {
        return await Store.GetObjectAsync<ObjectRegistration>(BuildRegistrationStateKey(id), ct);
    }
    
    public async Task<PutObjectResponse> StoreReceipt(ObjectRegistration state, CancellationToken ct = default)
    {
        return await Store.PutObjectAsync(BuildRegistrationStateKey(state.ObjectId), state, PutCondition.IfNoneExists, ct);
    }

}