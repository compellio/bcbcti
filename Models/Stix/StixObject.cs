using System.Text.Json;
using System.Text.Json.Serialization;
using BCBCTI.Models.Taxii;

namespace Bcbcti.Models.Stix;

// TODO FIXME Json derived classes - https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/polymorphism#serialize-properties-of-derived-classes
// TODO DANGER this affects STIX object serialization and hashing => public object URLs and checksums
[JsonDerivedType(typeof(StixBundle))]
[JsonDerivedType(typeof(StixUrlArtifact))]
[JsonDerivedType(typeof(StixObjectResource))] // TODO fixme bleeds from TAXII namespace
public abstract class StixObject
{
    protected StixObject(string id) => Id = id;
    
    public abstract string Type { get; }
    public string Id { get; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTime? Modified { get; set; }
    
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public DateTime? Created { get; set; }
    
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Payload { get; set; }
}