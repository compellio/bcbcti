namespace Bcbcti.Services.Storage.Models;

public class PutObjectRequest
{
    public required string ObjectKey { get; set; }
    public required Stream InputStream { get; set; }
    public byte[]? ChecksumSHA256 { get; set; }
}