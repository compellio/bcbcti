namespace Bcbcti.Models.Documents;

public class RegistrationMetadata
{
    public required int Version { get; set; }
    public required string RegistryChecksum { get; set; }
    
    public required string ObjectBundleId { get; set; }
    public required string ObjectArtifactId { get; set; }
}