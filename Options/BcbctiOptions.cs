using System.ComponentModel.DataAnnotations;

namespace Bcbcti.Options;

public class BcbctiOptions
{
    [Required]
    [MinLength(1)]
    public required CollectionOptions[] Collections { get; set; }
    
    
}