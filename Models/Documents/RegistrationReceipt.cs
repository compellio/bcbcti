namespace Bcbcti.Models.Documents;

public enum RegistrationReceiptState
{
    Sent,
    Succeeded,
    Failed
}

// TODO review location
public enum RegistryOperation
{
    Create, Update, Delete
}

public class RegistrationReceipt
{
    // System props
    public required Guid ReceiptId { get; set; }
    public required Guid JournalId { get; set; }
    public required Guid CollectionId { get; set; }

    // Registration props
    public required RegistryOperation Operation { get; set; }
    public required RegistrationReceiptState State { get; set; }

    // TAXII-relevant props
    /// <summary>
    /// The STIX Object identifier
    /// </summary>
    public required string ObjectId { get; set; }
    public required string ObjectKey { get; set; }
    /// <summary>
    /// The STIX Object version per TAXII's specification.
    /// </summary>
    /// <remarks>For STIX objects the version MUST be the STIX modified timestamp Property. If a STIX object is not versioned (and therefore does not have a modified timestamp), the server MUST use the created timestamp. If the STIX object does not have a created or modified timestamp then the server SHOULD use a value for the version that is consistent to the server.</remarks>
    public required DateTime ObjectVersion { get; set; }
    
    // History props
    public required DateTime SubmittedAt { get; set; } // when BCBCTI server received the object
    public required DateTime SentAt { get; set; } // when Registry API was called
    public DateTime? CompletedAt { get; set; } // when Registry API came back (webhook)
    
    public string? FailureReason { get; set; }

    public required RegistrationMetadata Metadata { get; set; }
}