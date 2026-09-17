namespace Bcbcti.Services.Storage.Models;

public class ListObjectRequest
{
    public required string Prefix { get; set; }
    public int? Limit { get; set; }
    public string? StartAfter { get; set; }
    public string? Cursor { get; init; }
}