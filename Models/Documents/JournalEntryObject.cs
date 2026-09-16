namespace Bcbcti.Models.Documents;

public class JournalEntryObject
{
    public required string Id { get; set; }
    public Guid? ReceiptId { get; set; }
    public string? ObjectKey { get; set; }
}