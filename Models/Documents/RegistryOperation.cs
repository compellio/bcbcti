using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Compellio.Bcbcti.Models.Documents;

public enum RegistryOperationType
{
    Create,
    Update,
    Delete
}

/// <summary>
/// Functions as a pending operation marker 
/// </summary>
public class RegistryOperation
{
    public required Guid CollectionId { get; set; }

    public required Guid JournalId { get; set; }
    public required DateTime SubmittedAt { get; set; } // used to calculate staleness
    
    public required string ObjectId { get; set; }
    public required RegistryOperationType OperationType { get; set; }

    public Guid? ReceiptId { get; set; }

    [JsonIgnore] 
    [MemberNotNullWhen(true, nameof(ReceiptId))]
    public bool WasSent => ReceiptId.HasValue;
    
    public RegistryOperation WithReceipt(Guid receiptId)
    {
        return new RegistryOperation()
        {
            CollectionId = CollectionId,
            ObjectId = ObjectId,
            JournalId = JournalId,
            SubmittedAt = SubmittedAt,
            OperationType = OperationType,
            
            ReceiptId = receiptId,
        };
    }
}