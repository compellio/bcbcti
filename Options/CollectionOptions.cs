using System.ComponentModel.DataAnnotations;

namespace Bcbcti.Options;

public class CollectionOptions
{
    [Required]
    public required Guid Id { get; set; }
    
    [Required]
    public required string Title { get; set; }
    
    public required string? Alias { get; set; }
    
    // public required CollectionStorageOptions Storage { get; set; }
}
