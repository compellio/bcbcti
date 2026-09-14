using System.ComponentModel.DataAnnotations;

namespace Bcbcti.Services.Storage.Providers.S3;

public class S3ObjectStoreOptions
{
    [Required]
    [MinLength(3)]
    // todo alphanumeric-dash validation
    public required string BucketName { get; set; }
    
    [Url]
    public string? PublicBaseUrl { get; set; }
}
