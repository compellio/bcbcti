namespace Bcbcti.Services.Storage.Models;

public class ListObjectResponse
{
    public required IReadOnlyList<ObjectMetadata> Objects { get; set; }
    public string? NextCursor { get; set; }
}