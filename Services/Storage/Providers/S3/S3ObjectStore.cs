using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Bcbcti.Services.Storage.Exceptions;
using Bcbcti.Services.Storage.Responses;

namespace Bcbcti.Services.Storage.Providers.S3;

// references
// https://github.com/awsdocs/aws-doc-sdk-examples/blob/main/dotnetv3/S3/scenarios/S3ConditionalRequestsScenario/S3ConditionalRequests/S3ActionsWrapper.cs
// https://github.com/dotnet/orleans/blob/76394f182bec081ba3fd1b0d4a912f1ea29746e3/src/AWS/Orleans.Journaling.S3/S3JournalStorage.cs

public class S3ObjectStore : IObjectStore
{
    private readonly IAmazonS3 _s3Client;
    private readonly S3ObjectStoreOptions _options;

    public S3ObjectStore(IAmazonS3 s3Client, S3ObjectStoreOptions options)
    {
        _s3Client = s3Client;
        _options = options;
    }

    private static bool IsObjectNotFound(AmazonS3Exception exception) =>
        exception.StatusCode == HttpStatusCode.NotFound &&
        !string.Equals(exception.ErrorCode, "NoSuchBucket", StringComparison.Ordinal);

    public async Task<GetResponse> GetObjectAsync(string objectKey, CancellationToken ct = default)
    {
        try
        {
            var request = new GetObjectRequest { BucketName = _options.BucketName, Key = objectKey };

            using var response = await _s3Client.GetObjectAsync(request, ct);
            
            using var payload = new MemoryStream();
            await response.ResponseStream.CopyToAsync(payload, ct);
            
            // todo read responsestream and cast into JSON object? should that be hapenning here (soft wrapper around S3, same behaviour for other stores => hints to the need of a common adapter)? -> failure mode?

            return new GetResponse
            {
                Body = payload, 
                ETag = response.ETag
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