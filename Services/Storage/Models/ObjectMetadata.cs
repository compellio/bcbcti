namespace Compellio.Bcbcti.Services.Storage.Models;

public class ObjectMetadata : ObjectSummary
{
    public required Uri PublicObjectUrl { get; set; }
    public string? ChecksumSha256 { get; set; }
}