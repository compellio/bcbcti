using Bcbcti.Services.Storage.Responses;

namespace Bcbcti.Services.Storage;

// TODO support write-ahead mutation conflicts (ETag, preconditions, etc.), e.g.: read a file before modifying it (e.g. /receipts/{receiptId}.json)
//   see UpdateMetadataAsync method in https://github.com/dotnet/orleans/blob/76394f182bec081ba3fd1b0d4a912f1ea29746e3/src/AWS/Orleans.Journaling.S3/S3JournalStorage.cs#L949

public interface IObjectStore
{
    public Uri GetObjectUri(string objectKey);
    
    public Task<PutResponse> PutObjectAsync(string objectKey, Stream input, CancellationToken ct = default);

    public Task<GetResponse<Stream>> GetObjectAsync(string objectKey, CancellationToken ct = default);

    // listAsync -> pagination w/ continuation token, date ordered!
    // deleteAsync
}