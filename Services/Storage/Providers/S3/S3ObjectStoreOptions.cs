using System.ComponentModel.DataAnnotations;

namespace Bcbcti.Services.Storage.Providers.S3;

public class S3ObjectStoreOptions
{
    [Required]
    [MinLength(3)]
    public required string BucketName { get; set; }
}