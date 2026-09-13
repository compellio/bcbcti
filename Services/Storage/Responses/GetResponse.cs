namespace Bcbcti.Services.Storage.Responses;

public class GetResponse
{
    public required Stream Body { get; set; }
    public string? ETag { get; set; }
}