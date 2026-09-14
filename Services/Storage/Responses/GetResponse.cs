namespace Bcbcti.Services.Storage.Responses;

public class GetResponse<TBody>
{
    public required TBody Body { get; set; }
    public required ObjectMetadata Metadata { get; set; }
}