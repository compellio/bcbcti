using Bcbcti.Models.Stix;
using Bcbcti.Services.Storage.Json;
using Bcbcti.Services.Storage.Results;

namespace Bcbcti.Repositories;

public class StixObjectRepository([FromKeyedServices("canonical")] IJsonObjectStore store) : Repository(store)
{
    
    private string BuildJournalKey(DateTime submissionDate, string id, DateTime version)
    {
        var partition = submissionDate.ToString("yyyy/MM/dd");
        var key = version.ToString("O");
        
        return $"objects/{partition}/{id}/{key}.json";
    }
    
    public async Task<GetObjectResult<StixObject>> GetJournalEntry(DateTime submissionDate, string id, DateTime version, CancellationToken ct = default)
    {
        return await Store.GetObjectAsync<StixObject>(BuildJournalKey(submissionDate, id, version), ct);
    }
    
    // TODO Store.ListObjects (submissionDate, id) = lists all versions for partition
    
    public async Task<PutObjectResult> PutJournalEntry(DateTime submissionDate, StixObject entry, CancellationToken ct = default)
    {
        // TODO perhaps move this somewhere more central?
        // This is per TAXII specifications (version = (stix.modified) else if (stix.created) else if (vendor-selection - we use submissionDate))
        var version = entry.Modified ?? entry.Created ?? submissionDate;
        
        // TODO FIXME BUG if 1+ objects with SAME id + no modified or created dates => clash => must be handled in validation
        
        // TODO FIXME BUG (submitting the same object)
        //      Fix = If-None-Match: * (412 if file exists, different create times => throw here)
        //      actually ... using submissionDate is an issue, because no duplicate objects w/o Modified or Created are not detected by If-None-Match: *
        //        => key content by hash? no duplication + If-None-Match: * will always throw by content (which incl. modified + created + object id)
        //           $"objects/{partition}/{id}/{modified}-{hash}.json"? sha256 or use another one?
        //           => compute sha256 externally and pass it down to S3, set ChecksumSHA256 in put request (currently S3 calculates it)
        
        return await Store.PutObjectAsync(BuildJournalKey(submissionDate, entry.Id, version), entry, ct);
    }
    
}