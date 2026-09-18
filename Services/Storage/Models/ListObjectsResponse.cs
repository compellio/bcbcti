namespace Compellio.Bcbcti.Services.Storage.Models;

public class ListObjectsResponse
{
    public required IAsyncEnumerable<ObjectMetadata> Objects { get; set; }
}