using System.ComponentModel.DataAnnotations;

namespace Compellio.Bcbcti.Models.Stix;

public class StixUrlArtifact(Guid id) : StixArtifact(id)
{
    [Url]
    public required Uri Url { get; set; }
    
    public required Dictionary<StixHashAlgorithm, string> Hashes { get; set; }
}