namespace Bcbcti.Services.Storage.Responses;

public class PutResponse
{
    // public required Stream Body { get; set; }
    public required Uri ObjectUrl { get; set; }
    public string? ETag { get; set; }
}