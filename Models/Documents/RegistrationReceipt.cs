namespace Bcbcti.Models.Documents;

public enum RegistrationReceiptState
{
    Sent,
    Succeeded,
    Failed
}

public class RegistrationReceipt
{
    public required Guid ReceiptId { get; set; }
    public required string ObjectKey { get; set; }
    
    public string? TarId { get; set; }
    
    public required DateTime SubmittedAt { get; set; } // when BCBCTI server received
    public required DateTime SentAt { get; set; } // when Registry API was called
    public DateTime? CompletedAt { get; set; } // when Registry API came back (webhook)
    
    public string? FailureReason { get; set; }
    
    public required RegistrationReceiptState State { get; set; }
    public required RegistrationMetadata Metadata { get; set; }
}