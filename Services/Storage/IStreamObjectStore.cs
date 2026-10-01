// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Services.Storage.Models;

namespace Compellio.Bcbcti.Services.Storage;

public interface IStreamObjectStore
{
    public Uri GetObjectUri(string objectKey);
    
    // TODO next iterations: hash algo should be configurable
    public Task<byte[]> ComputeSha256Hash(Stream input, CancellationToken ct = default);
    
    public Task<GetObjectResponse<Stream>> GetObjectAsync(string objectKey, CancellationToken ct = default);
    
    /// <summary>
    /// Similar to GetObjectAsync but returns null if no object is found
    /// </summary>
    public Task<GetObjectResponse<Stream>?> FindObjectAsync(string objectKey, CancellationToken ct = default);
    
    public Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request, CancellationToken ct = default);

    public Task<DeleteObjectResponse> DeleteObjectAsync(DeleteObjectRequest request, CancellationToken ct = default);

    /// <remarks>
    /// Modelled after S3, see https://docs.aws.amazon.com/AmazonS3/latest/API/API_ListObjectsV2.html
    /// 
    /// - Must return a single flat sequence of full keys (no nesting)
    /// - Returns objects in ascending lexicographical order based on their key names
    /// - If request.StartAfter is set, return only keys strictly greater than StartAfter
    /// - request.PageSize limits the entries returned per call (pagination is controlled with request.Cursor and response.NextCursor)
    /// </remarks>
    /// <example>
    /// <code>
    /// await store.ListObjectsAsync(new ListObjectRequest { Prefix = "manifest/2026/" });
    /// </code>
    /// will return the following keys:
    /// <code>
    /// [
    ///     "manifest/2026/09/15/2026-09-15T23:59:10.000Z-9d2f.json"
    ///     "manifest/2026/09/16/2026-09-16T08:01:02.500Z-1a77.json"
    ///     "manifest/2026/09/16/2026-09-16T12:03:20.000Z-4c8e.json"
    ///     "manifest/2026/09/17/2026-09-17T06:15:45.250Z-7b30.json"
    /// ]
    /// </code>
    /// and
    /// <code>
    /// await store.ListObjectsAsync(new ListObjectRequest { Prefix = "manifest/2026/", StartAfter = "manifest/2026/09/16/2026-09-16T08:01:02.500Z-1a77.json" });
    /// </code>
    /// will return the following keys:
    /// <code>
    /// [
    ///     "manifest/2026/09/16/2026-09-16T12:03:20.000Z-4c8e.json"
    ///     "manifest/2026/09/17/2026-09-17T06:15:45.250Z-7b30.json"
    /// ]
    /// </code>
    /// </example>
    public ListObjectsResponse ListObjectsAsync(ListObjectsRequest request);

}
