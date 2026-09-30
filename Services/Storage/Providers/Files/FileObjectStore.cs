using Compellio.Bcbcti.Services.Storage.Exceptions;
using Compellio.Bcbcti.Services.Storage.Models;
using System.Security.Cryptography;
using System.Text;

namespace Compellio.Bcbcti.Services.Storage.Providers.Files;

// TODO debug logging
public class FileObjectStore : IStreamObjectStore
{
    private readonly FileObjectStoreOptions _options;

    public FileObjectStore(FileObjectStoreOptions options)
    {
        _options = options;
    }

    private static string GetFileETag(string filePath)
    {
        var fileInfo = new FileInfo(filePath);
        string input = $"{fileInfo.LastWriteTimeUtc.Ticks}:{fileInfo.Length}";
        using var md5 = MD5.Create();

        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }

    private string ToFilePath(string objectKey)
    {
        var filePathOfObjectKey = objectKey.Replace('/', Path.DirectorySeparatorChar);
        var fullObjectPath = Path.Combine(_options.BaseFolder, filePathOfObjectKey);

        // TODO prevent path traversal (objectKey = ../../some-dir -> {_options.BaseFolder}/../../some-dir, leak)

        return fullObjectPath;
    }

    private string ToObjectKey(string filePath)
    {
        return Path.GetRelativePath(_options.BaseFolder, filePath).Replace(Path.DirectorySeparatorChar, '/');
    }

    public async Task<byte[]> ComputeSha256Hash(Stream input, CancellationToken ct = default)
    {
        var hash = await SHA256.HashDataAsync(input, ct);
        input.Position = 0;
        return hash;
    }

    public Uri GetObjectUri(string objectKey)
    {
        var builder = new UriBuilder(_options.BaseUri);

        var basePath = builder.Path.TrimEnd('/');
        var keyPath = objectKey.TrimStart('/');

        builder.Path = $"{basePath}/{keyPath}";

        return builder.Uri;
    }

    public async Task<GetObjectResponse<Stream>> GetObjectAsync(string objectKey, CancellationToken ct = default)
    {
        try
        {
            var objectPath = ToFilePath(objectKey);
            if (File.Exists(objectPath) == false)
            {
                throw new ObjectNotFoundException(objectPath);
            }

            using FileStream response = new FileStream(objectPath, FileMode.Open, FileAccess.Read);

            var payload = new MemoryStream();
            await response.CopyToAsync(payload, ct);
            payload.Position = 0;

            var sha256HashBuffer = await ComputeSha256Hash(payload, ct);

            return new GetObjectResponse<Stream>
            {
                Body = payload,
                Metadata = new ObjectMetadata
                {
                    PublicObjectUrl = GetObjectUri(objectKey),
                    ChecksumSha256 = Convert.ToBase64String(sha256HashBuffer),

                    ETag = GetFileETag(objectPath),
                    ObjectKey = objectKey,
                    LastModified = File.GetLastWriteTime(objectPath).ToUniversalTime(),
                }
            };
        }
        catch (ObjectNotFoundException)
        {
            throw;
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
            var objectPath = ToFilePath(request.ObjectKey);

            switch (request.Condition.Type)
            {
                case Condition.Kind.IfNotExists when File.Exists(objectPath):
                    throw new PutConditionException("File already exists");

                case Condition.Kind.IfMatch when File.Exists(objectPath) 
                                                 && request.Condition.ETag != GetFileETag(objectPath):
                    throw new PutConditionException("ETag mismatch");
            }

            var directoryPath = Path.GetDirectoryName(objectPath);
            if (directoryPath is not null)
            {
                Directory.CreateDirectory(directoryPath);
            }

            using FileStream response = new FileStream(objectPath, FileMode.Create, FileAccess.ReadWrite);

            await request.InputStream.CopyToAsync(response, ct);
            response.Position = 0;

            var hashBuffer = await ComputeSha256Hash(response, ct);

            return new PutObjectResponse
            {
                Metadata = new ObjectMetadata
                {
                    PublicObjectUrl = GetObjectUri(request.ObjectKey),
                    ChecksumSha256 = Convert.ToBase64String(hashBuffer),

                    ETag = GetFileETag(objectPath),
                    ObjectKey = request.ObjectKey,
                    LastModified = File.GetLastWriteTime(objectPath).ToUniversalTime(),
                }
            };
        }
        catch (PutConditionException)
        {
            throw;
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
            var jsonFiles = Directory.GetFiles(_options.BaseFolder, "*", SearchOption.AllDirectories);

            return new ListObjectsResponse
            {
                Objects = jsonFiles.Select(x => new ObjectSummary
                {
                    ETag = GetFileETag(x),
                    ObjectKey = ToObjectKey(x),
                    LastModified = File.GetLastWriteTime(x).ToUniversalTime(),
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
            var fullObjectPath = ToFilePath(request.ObjectKey);

            if (File.Exists(fullObjectPath) == false)
            {
                throw new ObjectNotFoundException(fullObjectPath);
            }

            File.Delete(fullObjectPath);

            return new DeleteObjectResponse
            {
                ObjectKey = request.ObjectKey,
            };
        }
        catch (ObjectNotFoundException)
        {
            throw;
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