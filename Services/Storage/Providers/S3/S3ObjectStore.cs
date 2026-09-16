using System.Net;
using System.Security.Cryptography;
using Amazon.S3;
using Bcbcti.Services.Storage.Exceptions;
using Bcbcti.Services.Storage.Results;

namespace Bcbcti.Services.Storage.Providers.S3;

// references - based on
// https://github.com/dotnet/orleans/blob/76394f182bec081ba3fd1b0d4a912f1ea29746e3/src/AWS/Orleans.Journaling.S3/S3JournalStorage.cs
// https://github.com/awsdocs/aws-doc-sdk-examples/blob/main/dotnetv3/S3/scenarios/S3ConditionalRequestsScenario/S3ConditionalRequests/S3ActionsWrapper.cs

// TODO debug logging?

public class S3ObjectStore : IStreamObjectStore
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3ObjectStoreOptions _options;

    private readonly Uri _publicBaseUri;

    public S3ObjectStore(IAmazonS3 s3Client, S3ObjectStoreOptions options, bool forcePathStyle = false)
    {
        _s3Client = s3Client;
        _options = options;
        _publicBaseUri = DerivePublicBaseUri(forcePathStyle);
    }

    /// <summary>
    /// Attempts to derive the public object URL based on existing configuration.
    /// The public object URL is included in registration payloads.
    /// </summary>
    /// <param name="forcePathStyle"></param>
    /// <returns></returns>
    /// <see href="https://docs.aws.amazon.com/AmazonS3/latest/userguide/VirtualHosting.html"/>
    /// <exception cref="InvalidOperationException">Thrown if unable to derive. Recommendation is to set PublicBaseUrl.</exception>
    private Uri DerivePublicBaseUri(bool forcePathStyle)
    {
        if (_options.PublicBaseUrl is not null)
        {
            if (!Uri.TryCreate(_options.PublicBaseUrl, UriKind.Absolute, out var uri))
            {
                throw new InvalidOperationException($"Invalid public base url: {_options.PublicBaseUrl}");
            }

            return uri;
        }

        if (_s3Client.Config.RegionEndpoint is not null)
        {
            var scheme = _s3Client.Config.UseHttp ? "http" : "https";
            var region = _s3Client.Config.RegionEndpoint.SystemName;
            var host = $"{_options.BucketName}.s3.{region}.amazonaws.com";

            var builder = new UriBuilder(scheme, host);

            if (forcePathStyle)
            {
                builder.Host = $"s3.{region}.amazonaws.com";
                builder.Path = $"{builder.Path.TrimEnd('/')}/{_options.BucketName}";
            }

            return builder.Uri;
        }

        if (Uri.TryCreate(_s3Client.Config.ServiceURL, UriKind.Absolute, out var serviceUri))
        {
            var builder = new UriBuilder(serviceUri);

            if (forcePathStyle)
            {
                builder.Path = $"{builder.Path.TrimEnd('/')}/{_options.BucketName}";
            }

            return builder.Uri;
        }

        throw new InvalidOperationException("Cannot derive public object URL.");
    }

    private static bool IsObjectNotFound(AmazonS3Exception exception) =>
        exception.StatusCode == HttpStatusCode.NotFound &&
        !string.Equals(exception.ErrorCode, "NoSuchBucket", StringComparison.Ordinal);

    public Uri GetObjectUri(string objectKey)
    {
        var builder = new UriBuilder(_publicBaseUri);

        var basePath = builder.Path.TrimEnd('/');
        var keyPath = objectKey.TrimStart('/');

        builder.Path = $"{basePath}/{keyPath}";

        return builder.Uri;
    }

    public async Task<byte[]> ComputeSha256Hash(Stream input, CancellationToken ct = default)
    {
        var hash = await SHA256.HashDataAsync(input, ct);
        input.Position = 0;
        return hash;
    }

    public async Task<GetObjectResult<Stream>> GetObjectAsync(string objectKey, CancellationToken ct = default)
    {
        try
        {
            var request = new Amazon.S3.Model.GetObjectRequest { BucketName = _options.BucketName, Key = objectKey };

            // TODO etag headers

            using var response = await _s3Client.GetObjectAsync(request, ct);

            var payload = new MemoryStream();
            await response.ResponseStream.CopyToAsync(payload, ct);
            payload.Position = 0;

            return new GetObjectResult<Stream>
            {
                Body = payload,
                Metadata = new ObjectMetadata
                {
                    ObjectKey = objectKey,
                    PublicObjectUrl = GetObjectUri(objectKey),
                    ChecksumSha256 = response.ChecksumSHA256,
                    ETag = response.ETag
                }
            };
        }
        catch (AmazonS3Exception e) when (IsObjectNotFound(e)) // TODO FIXME S3 PutObject only throws bucket not found
        {
            throw new ObjectNotFoundException(e.Message, e);
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

    public Task<PutObjectResponse> PutObjectAsync(string objectKey, Stream input, CancellationToken ct = default) =>
        PutObjectAsync(new PutObjectRequest { ObjectKey = objectKey, InputStream = input }, ct);

    public async Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request, CancellationToken ct = default)
    {
        try
        {
            var s3Request = new Amazon.S3.Model.PutObjectRequest
            {
                BucketName = _options.BucketName,
                Key = request.ObjectKey,
                InputStream = request.InputStream,
                AutoCloseStream = false,
                UseChunkEncoding = false,
            };

            if (request.ChecksumSHA256 is not null)
            {
                // Set verification checksum manually
                s3Request.ChecksumSHA256 = Convert.ToBase64String(request.ChecksumSHA256);
            }
            else
            {
                // Let S3 compute and verify the checksum
                s3Request.ChecksumAlgorithm = ChecksumAlgorithm.SHA256;
            }

            // TODO etag headers + potentially add metadata, etc.

            var response = await _s3Client.PutObjectAsync(s3Request, ct);

            return new PutObjectResponse
            {
                Metadata = new ObjectMetadata
                {
                    ObjectKey = request.ObjectKey,
                    PublicObjectUrl = GetObjectUri(request.ObjectKey),
                    ChecksumSha256 = response.ChecksumSHA256,
                    ETag = response.ETag
                }
            };
        }
        catch (AmazonS3Exception e) when (IsObjectNotFound(e))
        {
            throw new ObjectNotFoundException(e.Message, e);
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