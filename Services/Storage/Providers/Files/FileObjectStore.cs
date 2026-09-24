using Amazon.S3;
using Compellio.Bcbcti.Services.Storage.Exceptions;
using Compellio.Bcbcti.Services.Storage.Models;
using System.Drawing;
using System.Net;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Compellio.Bcbcti.Services.Storage.Providers.Files;

// TODO debug logging
public class FileObjectStore : IStreamObjectStore
{
    private readonly FileObjectStoreOptions _options;

    private readonly string _publicBaseUri;

    public FileObjectStore(FileObjectStoreOptions options)
    {
        _options = options;
        _publicBaseUri = "file://localhost/";
    }

    private static bool IsObjectNotFound(AmazonS3Exception exception) =>
        exception.StatusCode == HttpStatusCode.NotFound &&
        !string.Equals(exception.ErrorCode, "NoSuchBucket", StringComparison.Ordinal);

    private static bool IsConditionConflict(AmazonS3Exception exception) =>
        exception.StatusCode is HttpStatusCode.PreconditionFailed or HttpStatusCode.Conflict;

    public Uri GetObjectUri(string objectKey)
    {
        var builder = new UriBuilder(_publicBaseUri);

        var basePath = builder.Path.TrimEnd('/');
        var keyPath = objectKey.TrimStart('/').Concat(".json");

        builder.Path = $"{basePath}/{keyPath}";

        return builder.Uri;
    }

    public async Task<byte[]> ComputeSha256Hash(Stream input, CancellationToken ct = default)
    {
        var hash = await SHA256.HashDataAsync(input, ct);
        input.Position = 0;
        return hash;
    }

    public async Task<GetObjectResponse<Stream>> GetObjectAsync(string objectKey, CancellationToken ct = default)
    {
        try
        {
            var fullObjectPath = Path.Combine(_options.BaseFolder, objectKey, ".json");
            if (File.Exists(fullObjectPath) == false)
            {
                throw new ObjectNotFoundException(fullObjectPath, null);
            }

            var lastModified = File.GetLastWriteTime(fullObjectPath);
            using FileStream response = new FileStream(fullObjectPath, FileMode.Open, FileAccess.Read);

            var payload = new MemoryStream();
            await response.CopyToAsync(payload, ct);
            payload.Position = 0;

            return new GetObjectResponse<Stream>
            {
                Body = payload,
                Metadata = new ObjectMetadata
                {
                    ObjectKey = objectKey,
                    PublicObjectUrl = GetObjectUri(objectKey),
                    ChecksumSha256 = null,
                    ETag = null,
                    LastModified = lastModified
                }
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new ProviderOperationException(e.Message, e);
        }
    }

    public async Task<GetObjectResponse<Stream>?> FindObjectAsync(string objectKey, CancellationToken ct = default)
    {
        try
        {
            return await GetObjectAsync(objectKey, ct);
        }
        catch (ObjectNotFoundException)
        {
            return null;
        }
    }

    public async Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request, CancellationToken ct = default)
    {
        try
        {
            var fullObjectPath = Path.Combine(_options.BaseFolder, request.ObjectKey, ".json");
            using FileStream response = new FileStream(fullObjectPath, FileMode.Create, FileAccess.ReadWrite);

            await request.InputStream.CopyToAsync(response, ct);
            response.Position = 0;

            var hashBuffer = await ComputeSha256Hash(response, ct);

            return new PutObjectResponse
            {
                Metadata = new ObjectMetadata
                {
                    ObjectKey = request.ObjectKey,
                    PublicObjectUrl = GetObjectUri(request.ObjectKey),
                    ChecksumSha256 = BitConverter.ToString(hashBuffer),
                    ETag = null,
                }
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new ProviderOperationException(e.Message, e);
        }
    }

    public ListObjectsResponse ListObjectsAsync(ListObjectsRequest request)
    {
        try
        {
            var jsonFiles = Directory.GetFiles(_options.BaseFolder, "*.json");

            var s3ListRequest = new Amazon.S3.Model.ListObjectsV2Request
            {
                BucketName = _options.BaseFolder,
                Prefix = request.Prefix,
                StartAfter =  request.StartAfter
            };

            return new ListObjectsResponse
            {
                Objects = jsonFiles.Select(x => new ObjectSummary
                {
                    ETag = null,
                    ObjectKey = Path.GetFileNameWithoutExtension(x),
                    LastModified = File.GetLastWriteTime(x),
                })
                .Where(x => x.ObjectKey != null && (request.StartAfter != null ? string.Compare(x.ObjectKey, request.StartAfter) > 0 : true) && (string.IsNullOrEmpty(request.Prefix) ? true : x.ObjectKey.StartsWith(request.Prefix)))
                .ToAsyncEnumerable()
            };
        }
        catch (Exception e)
        {
            // TODO FIXME DANGER lazy IAsyncEnumeration => won't throw here
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<DeleteObjectResponse> DeleteObjectAsync(DeleteObjectRequest request, CancellationToken ct = default)
    {
        try
        {
            var fullObjectPath = Path.Combine(_options.BaseFolder, request.ObjectKey, ".json");
            if (File.Exists(fullObjectPath) == false)
            {
                throw new ObjectNotFoundException(fullObjectPath, null);
            }

            File.Delete(fullObjectPath);
            
            return new DeleteObjectResponse
            {
                ObjectKey = request.ObjectKey,
            };
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception e)
        {
            throw new ProviderOperationException(e.Message, e);
        }
    }
    
}