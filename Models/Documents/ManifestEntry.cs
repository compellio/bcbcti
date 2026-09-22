namespace Compellio.Bcbcti.Models.Documents;

public class ManifestEntry
{
    public required string ObjectId { get; set; }
    public required string ObjectKey { get; set; }
    
    public required Guid ReceiptId { get; set; }
    public required DateTime CompletedAt { get; set; } // = TAXII DateAdded
    
    public required RegistrationMetadata RegistrationMetadata { get; set; } // TODO FIXME issue: TarId required in this case
}