namespace Bcbcti.Options;

public class StorageOptions
{
    public required string BucketName { get; set; }
    public string? Region { get; set; }
    public string? ServiceUrl { get; set; }
    public bool ForcePathStyle { get; set; }
    public string? KeyPrefix { get; set; }
}
