using System.Text.Json.Serialization;
using Bcbcti.Models.Stix;

namespace BCBCTI.Models.Taxii;

public class StixObjectResource : StixObject
{
    [JsonConstructor]
    public StixObjectResource(string type, string id) : base(id)
    {
        Type = type;
    }

    public override string Type { get; }
}