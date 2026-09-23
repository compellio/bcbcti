namespace Compellio.Bcbcti.Models.Documents;

// TODO FIXME this is not a stored document but part of RegistrationReceipt and ManifestEntry
public class VersionMetadata
{
    public required int Version { get; set; }
    public required string RegistryChecksum { get; set; }
}