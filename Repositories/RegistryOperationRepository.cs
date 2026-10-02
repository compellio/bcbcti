// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Services.Storage.Json;
using Compellio.Bcbcti.Services.Storage.Models;

namespace Compellio.Bcbcti.Repositories;

public class RegistryOperationRepository(IJsonObjectStore store) : Repository(store)
{
    private const string Prefix = "pending-operations/";

    private string BuildOperationKey(string objectId)
    {
        return $"{Prefix}{objectId}.json";
    }

    public async Task<GetObjectResponse<RegistryOperation>> GetRegistryOperationByKey(string key,
        CancellationToken ct = default)
    {
        return await Store.GetObjectAsync<RegistryOperation>(key, ct);
    }

    // TODO differentiate between GetRegistryOperationByKey-ByKey vs -ByObjectId, c# convention (same string signature - no overload)?
    public async Task<GetObjectResponse<RegistryOperation>> GetRegistryOperation(string objectId, CancellationToken ct = default)
    {
        return await Store.GetObjectAsync<RegistryOperation>(BuildOperationKey(objectId), ct);
    }

    public async Task<GetObjectResponse<RegistryOperation>?> FindRegistryOperation(string objectId,
        CancellationToken ct = default)
    {
        return await Store.FindObjectAsync<RegistryOperation>(BuildOperationKey(objectId), ct);
    }

    public async Task<PutObjectResponse> CreateRegistryOperation(RegistryOperation entry,
        CancellationToken ct = default)
    {
        return await Store.PutObjectAsync(BuildOperationKey(entry.ObjectId), entry,
            Condition.IfNoneExists, ct);
    }

    public async Task<PutObjectResponse> UpdateRegistryOperation(string etag, RegistryOperation entry,
        CancellationToken ct = default)
    {
        return await Store.PutObjectAsync(BuildOperationKey(entry.ObjectId), entry,
            Condition.IfMatch(etag), ct);
    }

    public ListObjectsResponse ListRegistryOperations(string? startAfter = null)
    {
        return Store.ListObjectsAsync(Prefix, startAfter);
    }

    public async Task<DeleteObjectResponse> DeleteRegistryOperation(string objectId, CancellationToken ct = default)
    {
        return await Store.DeleteObjectAsync(BuildOperationKey(objectId), ct);
    }

    public async Task<DeleteObjectResponse> DeleteRegistryOperationByKey(string objectKey,
        CancellationToken ct = default)
    {
        return await Store.DeleteObjectAsync(objectKey, ct);
    }
}