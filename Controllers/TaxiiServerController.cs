using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Controllers;

[ApiController]
[Route("/")]
[Consumes("application/taxii+json", "application/taxii+json;version=2.1")]
[Produces("application/taxii+json;version=2.1")]
public class TaxiiServerController(
    IOptions<TaxiiOptions> options,
    JournalRepository journalRepository,
    RegistrationReceiptsRepository receiptsRepository) : ControllerBase
{
    [HttpGet(Name = "TaxiiServerDiscovery")]
    [Route("/taxii2")]
    public DiscoveryResource Discovery()
    {
        return new DiscoveryResource { Title = options.Value.ServerTitle, Default = "/api/", ApiRoots = ["/api/"] };
    }

    [HttpGet(Name = "TaxiiApiRootInformation")]
    [Route("/api")]
    public ApiRootResource RootInformation()
    {
        return new ApiRootResource
        {
            Title = options.Value.Title,
            Description = options.Value.Description,
            Versions = ["application/taxii+json;version=2.1"],
            MaxContentLength = (int)options.Value.MaxUploadBytes,
        };
    }

    [HttpGet(Name = "TaxiiApiRootStatus")]
    [Route("/api/status/{id:guid}")]
    public async Task<StatusResource> StatusInformation(Guid id, CancellationToken ct)
    {
        // TODO FIXME catch ObjectNotFoundException -> turn into TAXII 404
        var journal = await journalRepository.GetJournalEntry(id, ct);

        // TODO FIXME to dictionary mapping (no O(n^2) with receipts.FirstOrDefault)
        var receipts = await receiptsRepository.FindReceipts(
            journal.Body.Objects.Where(o => !o.HasFailedEarly).Select(o => o.ReceiptId!.Value).ToArray(), ct);

        var failures = new List<StatusDetailsResource>();
        var successes = new List<StatusDetailsResource>();
        var pendings = new List<StatusDetailsResource>();

        foreach (var journalObject in journal.Body.Objects)
        {
            var stixObject = new StatusDetailsResource
            {
                Id = journalObject.ObjectId,
                Version = journalObject.ObjectVersion,
                ReceiptId = journalObject.ReceiptId,
            };
            
            if (journalObject.HasFailedEarly)
            {
                stixObject.Message = journalObject.SubmitFailureReason;
                failures.Add(stixObject);
            }
            else
            {
                var receipt = receipts.FirstOrDefault(o => o?.Body.ReceiptId == journalObject.ReceiptId.Value);

                switch (receipt?.Body.State)
                {
                    case RegistrationReceiptState.Succeeded:
                        successes.Add(stixObject);
                        break;
                    
                    case RegistrationReceiptState.Sent:
                        pendings.Add(stixObject);
                        break;
                    
                    default:
                        stixObject.Message = journalObject.SubmitFailureReason;
                        failures.Add(stixObject);
                        break;
                }
            }
        }

        return new StatusResource
        {
            Id = journal.Body.Id,
            RequestTimestamp = journal.Body.RequestTimestamp,
            TotalCount = journal.Body.Objects.Length,
            Status = pendings.Count > 0 ? StatusValue.Pending : StatusValue.Complete,
            FailureCount = failures.Count,
            Failures = failures.ToArray(),
            SuccessCount = successes.Count,
            Successes = successes.ToArray(),
            PendingCount = pendings.Count,
            Pendings = pendings.ToArray(),
        };
    }
}