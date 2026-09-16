using Bcbcti.Services.Storage.Models;

namespace Bcbcti.Services.Storage;

// TODO support write-ahead mutation conflicts (ETag, preconditions, etc.), e.g.: read a file before modifying it (e.g. /receipts/{receiptId}.json)
//   see UpdateMetadataAsync method in https://github.com/dotnet/orleans/blob/76394f182bec081ba3fd1b0d4a912f1ea29746e3/src/AWS/Orleans.Journaling.S3/S3JournalStorage.cs#L949

public interface IStreamObjectStore
{
    public Uri GetObjectUri(string objectKey);
    
    // TODO next iterations: hash algo should be configurable
    public Task<byte[]> ComputeSha256Hash(Stream input, CancellationToken ct = default);
    
    // TODO GetObjectRequest w/ condition options, etc.
    public Task<GetObjectResponse<Stream>> GetObjectAsync(string objectKey, CancellationToken ct = default);
    
    public Task<PutObjectResponse> PutObjectAsync(PutObjectRequest request, CancellationToken ct = default);

    // listAsync -> pagination w/ continuation token, date ordered!
    // deleteAsync
}