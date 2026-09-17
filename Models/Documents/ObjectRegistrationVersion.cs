namespace Bcbcti.Models.Documents;

public class ObjectRegistrationVersion
{
    public required Guid ReceiptId { get; set; }
    
    public required string ObjectKey { get; set; }
    
    public required DateTime ObjectVersion { get; set; }
    public required int TarVersion { get; set; }
    
    public required DateTime CompletedAt { get; set; } // = TAXII DateAdded
}