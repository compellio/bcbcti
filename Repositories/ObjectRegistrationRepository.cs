using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Services.Storage.Json;
using Compellio.Bcbcti.Services.Storage.Models;

namespace Compellio.Bcbcti.Repositories;

public class ObjectRegistrationRepository(IJsonObjectStore store) : Repository(store)
{

    private string BuildObjectRegistrationKey(string id)
    {
        return $"registrations/{id}.json";
    }
    
    public async Task<GetObjectResponse<ObjectRegistration>> GetObjectRegistration(string objectId, CancellationToken ct = default)
    {
        return await Store.GetObjectAsync<ObjectRegistration>(BuildObjectRegistrationKey(objectId), ct);
    }
    
    public async Task<GetObjectResponse<ObjectRegistration>?> FindObjectRegistration(string objectId, CancellationToken ct = default)
    {
        return await Store.FindObjectAsync<ObjectRegistration>(BuildObjectRegistrationKey(objectId), ct);
    }
    
    public async Task<PutObjectResponse> StoreObjectRegistration(ObjectRegistration state, CancellationToken ct = default)
    {
        return await Store.PutObjectAsync(BuildObjectRegistrationKey(state.ObjectId), state, PutCondition.IfNoneExists, ct);
    }
    
    public async Task<PutObjectResponse> PutObjectRegistration(string etag, ObjectRegistration state, CancellationToken ct = default)
    {
        return await Store.PutObjectAsync(BuildObjectRegistrationKey(state.ObjectId), state, PutCondition.IfMatch(etag), ct);
    }

}