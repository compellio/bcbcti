using System.ComponentModel.DataAnnotations;

namespace Compellio.Bcbcti.Services.Storage.Providers.Files;

public class FileObjectStoreOptions
{
    [Required]
    [MinLength(3)]
    // TODO name validation with System.ComponentModel.DataAnnotations.RegularExpression
    //      see https://docs.aws.amazon.com/AmazonS3/latest/userguide/bucketnamingrules.html#general-purpose-bucket-names
    public required string BaseUri { get; set; }

    [Required]
    [MinLength(3)]
    // TODO name validation with System.ComponentModel.DataAnnotations.RegularExpression
    //      see https://docs.aws.amazon.com/AmazonS3/latest/userguide/bucketnamingrules.html#general-purpose-bucket-names
    public required string BaseFolder { get; set; }
}
