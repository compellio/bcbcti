namespace Bcbcti.Services.Storage.Results;

public class PutObjectRequest
{
    public required string ObjectKey { get; set; }
    public required Stream InputStream { get; set; }
    public byte[]? ChecksumSHA256 { get; set; }
}