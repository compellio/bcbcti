namespace Bcbcti.Services.Storage;

public interface IObjectStore
{
    // interface for S3/AzureBlob/...
    
    // putAsync -> caution w/ receipt documents (concurrency) => ETag condition!!
    // getAsync --> ETag condition!
    // listAsync -> pagination w/ continuation token, date ordered!
    // deleteAsync
}