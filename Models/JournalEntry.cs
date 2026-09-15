namespace Bcbcti.Models;

public class JournalEntry
{
    
    public required Guid Id { get; set; }
    public required Guid CollectionId { get; set; }
    public required DateTime RequestTimestamp { get; set; }
    public required JournalEntryObject[] Objects { get; set; }
}