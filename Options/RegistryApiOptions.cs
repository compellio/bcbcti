namespace Bcbcti.Options;

public class RegistryApiOptions
{
    public required string ApiKey { get; set; }
    public string? ServiceUrl { get; set; }
    public string? Network { get; set; }
    public string? IssuerDomain { get; set; }
    public string? WebhookSecret { get; set; }
}