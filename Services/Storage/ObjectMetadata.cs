namespace Bcbcti.Services.Storage;

public class ObjectMetadata
{
    public string? ETag { get; set; }
    public required Uri PublicObjectUrl { get; set; }
}