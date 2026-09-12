using System.Text.Json;
using System.Text.Json.Serialization;

namespace Bcbcti.Models.Stix;

public class StixObjectResource
{
    
    public required string Type { get; set; }
    public required string Id { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Payload { get; set; }
    
}