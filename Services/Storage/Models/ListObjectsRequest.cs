namespace Compellio.Bcbcti.Services.Storage.Models;

public class ListObjectsRequest
{
    public required string Prefix { get; set; }
    public string? StartAfter { get; set; }
}