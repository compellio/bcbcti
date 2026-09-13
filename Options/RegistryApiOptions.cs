using System.ComponentModel.DataAnnotations;

namespace Bcbcti.Options;

public class RegistryApiOptions
{
    [Required]
    public required string ApiKey { get; set; }
    
    [Required, Url]
    public string? ServiceUrl { get; set; }
    
    public string? Network { get; set; }
    public string? IssuerDomain { get; set; }
    public string? WebhookSecret { get; set; }
}