namespace Bcbcti.Services.Storage.Results;

public class GetObjectResult<TBody>
{
    public required TBody Body { get; set; }
    public required ObjectMetadata Metadata { get; set; }
}