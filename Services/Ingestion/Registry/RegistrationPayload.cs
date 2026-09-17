using Compellio.Bcbcti.Models.Stix;

namespace Compellio.Bcbcti.Services.Ingestion.Registry;

public class RegistrationPayload
{
    public required StixArtifact Artifact { get; set; }
    public required StixBundle Bundle { get; set; }
    public required TarPayload Payload { get; set; }
};