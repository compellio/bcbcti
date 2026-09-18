using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Models.Stix;

namespace Compellio.Bcbcti.Services.Ingestion;

public enum IngestionResultResolution
{
    Success,
    Failure,
    Abort // pending registration
}

public class StixIngestionResult
{
    public required IngestionResultResolution Resolution { get; set; } // TODO FIXME don't like it: conditional props based on Resolution
    public required StixObject StixObject { get; set; }

    public RegistrationReceipt? RegistrationReceipt { get; set; }
    
    public string? ResolutionFailureMessage { get; set; }
    
    // public string? ObjectKey { get; set; }
    // public Guid? ReceiptId { get; set; }
    // public RegistrationPayload? Payload { get; set; }
}
