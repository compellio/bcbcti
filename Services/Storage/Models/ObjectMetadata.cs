namespace Bcbcti.Services.Storage.Models;

public class ObjectMetadata
{
    public required string ObjectKey { get; set; }
    public required Uri PublicObjectUrl { get; set; }
    public required string ETag { get; set; }
    public required string ChecksumSha256 { get; set; }
    public DateTime? LastModified { get; set; }
}