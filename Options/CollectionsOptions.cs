using System.ComponentModel.DataAnnotations;

namespace Compellio.Bcbcti.Options;

public class CollectionsOptions
{
    [Required]
    [MinLength(1)]
    public required CollectionOptions[] Collections { get; set; }
}