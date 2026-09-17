namespace Compellio.Bcbcti.Services.Storage.Models;

public class GetObjectResponse<TBody>
{
    public required TBody Body { get; set; }
    public required ObjectMetadata Metadata { get; set; }
}