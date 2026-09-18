namespace Compellio.Bcbcti.Services.Storage.Models;

public class ListObjectsResponse
{
    public required IAsyncEnumerable<ObjectSummary> Objects { get; set; }
}