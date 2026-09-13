using Bcbcti.Services.Storage.Responses;

namespace Bcbcti.Services.Storage;

// TODO support write-ahead mutation conflicts (ETag, preconditions, etc.), e.g.: read a file before modifying it (e.g. /receipts/{receiptId}.json)
//   see UpdateMetadataAsync method in https://github.com/dotnet/orleans/blob/76394f182bec081ba3fd1b0d4a912f1ea29746e3/src/AWS/Orleans.Journaling.S3/S3JournalStorage.cs#L949

public interface IObjectStore
{
    // interface for S3/AzureBlob/...

    // putAsync -> caution w/ receipt documents (concurrency) => ETag condition!!

    public Task<GetResponse> GetObjectAsync(string objectKey, CancellationToken ct = default);

    // listAsync -> pagination w/ continuation token, date ordered!
    // deleteAsync
}