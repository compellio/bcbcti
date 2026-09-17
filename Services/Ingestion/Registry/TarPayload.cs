using System.Text.Json.Serialization;
using Compellio.Bcbcti.Models.Stix;

namespace Compellio.Bcbcti.Services.Ingestion.Registry;

public class TarPayload
{
    [JsonPropertyName("@context")]
    public required string JsonLdContext { get; set; }

    [JsonPropertyName("@type")] 
    public required string JsonLdType { get; set; }

    public required StixBundle Bundle { get; set; }
}