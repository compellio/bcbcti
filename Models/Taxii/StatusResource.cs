// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Models.Taxii;

public enum StatusValue { Complete, Pending }

/// <summary>
/// TAXII 4.3.1 Status Resource
/// </summary>
/// <see href="https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285806"/>
public class StatusResource
{
    /// <summary>
    /// The identifier of this Status resource.
    /// </summary>
    public required Guid Id  { get; set; }
    
    /// <summary>
    /// The overall status of a previous POST request where an HTTP 202 (Accept) was returned. The value of this property MUST be one of complete or pending. A value of complete indicates that this resource will not be updated further, and MAY be removed in the future. A status of pending indicates that this resource MAY be updated in the future.
    /// </summary>
    public required StatusValue Status { get; set; }

    /// <summary>
    /// The datetime of the request that this status resource is monitoring.
    /// </summary>
    public DateTime? RequestTimestamp { get; set; }

    /// <summary>
    /// The total number of objects that were in the request, which would be the number of objects in the envelope. The value of the total_count MUST be a positive integer greater than or equal to zero. If this property has a value of 0, then the TAXII Server has not yet started processing the request.
    /// </summary>
    public required int TotalCount { get; set; }
    
    /// <summary>
    /// The number of objects that were successfully created. The value of the `success_count` MUST be a positive integer greater than or equal to zero.
    /// </summary>
    public required int SuccessCount { get; set; }
    
    /// <summary>
    /// A list of objects that was successfully processed.
    /// </summary>
    public StatusDetailsResource[]? Successes { get; set; }
    
    /// <summary>
    /// The number of objects that failed to be created. The value of the `failure_count` MUST be a positive integer greater than or equal to zero.
    /// </summary>
    public required int FailureCount { get; set; }
    
    /// <summary>
    /// A list of objects that was not successfully processed.
    /// </summary>
    public StatusDetailsResource[]? Failures { get; set; }
    
    /// <summary>
    /// The number of objects that have yet to be processed. The value of the pending_count MUST be a positive integer greater than or equal to zero.
    /// </summary>
    public required int PendingCount { get; set; }
    
    /// <summary>
    /// A list of objects that have yet to be processed.
    /// </summary>
    public StatusDetailsResource[]? Pendings { get; set; }

}