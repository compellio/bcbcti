namespace Bcbcti.Services.Ingestion;

public enum IngestionResultResolution
{
    Success,
    Failure
}

public class StixIngestionResult
{
    public required string? ObjectKey { get; set; }
    public required Guid? ReceiptId { get; set; }
    
    // metadata (receipt id, etc.)
}