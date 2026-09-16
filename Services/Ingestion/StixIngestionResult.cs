using Bcbcti.Services.Ingestion.Registry;

namespace Bcbcti.Services.Ingestion;

public enum IngestionResultResolution
{
    Success,
    Failure
}

public class StixIngestionResult
{
    public string? ObjectKey { get; set; }
    public Guid? ReceiptId { get; set; }
    public TarPayload? TarPayload { get; set; }
}