using System.Text.Json.Serialization;
using Compellio.Bcbcti.Models.Stix;

namespace Compellio.Bcbcti.Models.Taxii;

public class StixObjectResource : StixObject
{
    [JsonConstructor]
    public StixObjectResource(string type, string id) : base(id)
    {
        Type = type;
    }

    public override string Type { get; }
}