using Bcbcti.Models.Documents;
using Bcbcti.Services.Storage.Json;
using Bcbcti.Services.Storage.Models;

namespace Bcbcti.Repositories;

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