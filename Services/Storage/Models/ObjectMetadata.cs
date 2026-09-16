namespace Bcbcti.Services.Storage.Models;

public class ObjectMetadata
{
    public string? ETag { get; set; }
    public required string ChecksumSha256 { get; set; }
    public required Uri PublicObjectUrl { get; set; }
    public required string ObjectKey { get; set; }
}