using System.ComponentModel.DataAnnotations;

namespace Compellio.Bcbcti.Services.Storage.Providers.S3;

public class S3ObjectStoreOptions
{
    [Required]
    [MinLength(3)]
    // TODO name validation with System.ComponentModel.DataAnnotations.RegularExpression
    //      see https://docs.aws.amazon.com/AmazonS3/latest/userguide/bucketnamingrules.html#general-purpose-bucket-names
    public required string BucketName { get; set; }
    
    [Url]
    public string? PublicBaseUrl { get; set; }
}
