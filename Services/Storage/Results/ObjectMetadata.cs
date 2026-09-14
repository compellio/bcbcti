namespace Bcbcti.Services.Storage.Results;

public class ObjectMetadata
{
    public string? ETag { get; set; }
    public required Uri PublicObjectUrl { get; set; }
}