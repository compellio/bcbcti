// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Models.Taxii;

namespace Compellio.Bcbcti.Services.Taxii.Mappers;

public static class StatusMapper
{

    public static StatusResource ToResource(JournalEntry journal, IDictionary<Guid, RegistrationReceipt> receipts)
    {
        var failures = new List<StatusDetailsResource>();
        var successes = new List<StatusDetailsResource>();
        var pendings = new List<StatusDetailsResource>();
        
        foreach (var journalObject in journal.Objects)
        {
            var details = new StatusDetailsResource
            {
                Id = journalObject.ObjectId,
                Version = journalObject.ObjectVersion,
                ReceiptId = journalObject.ReceiptId,
            };

            if (journalObject.HasFailedEarly)
            {
                details.Message = journalObject.SubmitFailureReason;
                failures.Add(details);
            }
            else
            {
                receipts.TryGetValue(journalObject.ReceiptId.Value, out var receipt);

                switch (receipt?.State)
                {
                    case RegistrationReceiptState.Succeeded:
                        successes.Add(details);
                        break;

                    case RegistrationReceiptState.Failed:
                        details.Message = receipt.FailureReason;
                        failures.Add(details);
                        break;

                    default:
                        // considering null receipts as pending (not completed)
                        pendings.Add(details);
                        break;
                }
            }
        }
        
        return new StatusResource
        {
            Id = journal.Id,
            RequestTimestamp = journal.RequestTimestamp,
            TotalCount = journal.Objects.Length,
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