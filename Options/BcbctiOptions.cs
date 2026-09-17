using System.ComponentModel.DataAnnotations;

namespace Compellio.Bcbcti.Options;

public class BcbctiOptions
{
    [Required]
    [MinLength(1)]
    public required CollectionOptions[] Collections { get; set; }
    
    
}