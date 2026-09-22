using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Models.Stix;
using Compellio.Bcbcti.Services.Ingestion.Registry;
using Compellio.Bcbcti.Services.RegistryApi.Models;
using Compellio.Bcbcti.Services.RegistryApi.Profiles;
using Compellio.Bcbcti.Services.Storage.Models;
using UUIDNext;

namespace Compellio.Bcbcti.Services.Ingestion;

public class RegistrationPayloadFactory
{
    public TarPayload BuildTarPayload(StixBundle bundle)
    {
        return new TarPayload
        {
            JsonLdContext = StixBundleProfileV1.ProfileId,
            JsonLdType = StixBundleProfileV1.Type,
            Version = "2.1",
            Bundle = bundle
        };
    }

    public StixBundle BuildStixBundle(StixObject[] objects)
    {
        var bundleGuid = Guid.NewGuid();

        return new StixBundle(bundleGuid) { Objects = objects };
    }

    public StixArtifact BuildStixArtifact(ObjectMetadata metadata)
    {
        // TODO UUID calculation is something that could/should be encapsulated -> StixUrlArtifact leaks logic BUT UUIDv5 is optional => constructor id should be optional as well
        var artifactGuid =
            Uuid.NewNameBased(StixConstants.ScoIdentifierUuid5Namespace, metadata.PublicObjectUrl.ToString());

        return new StixUrlArtifact(artifactGuid)
        {
            Url = metadata.PublicObjectUrl,
            MimeType = StixConstants.MediaType,
            Hashes = new Dictionary<StixHashAlgorithm, string>
            {
                { StixHashAlgorithm.SHA_256, metadata.ChecksumSha256 }
            }
        };
    }

    public RegistrationPayload BuildRegistrationPayload(ObjectMetadata metadata)
    {
        var stixArtifact = BuildStixArtifact(metadata);
        var stixBundle = BuildStixBundle([stixArtifact]);
        var tarPayload = BuildTarPayload(stixBundle);

        return new RegistrationPayload { Artifact = stixArtifact, Bundle = stixBundle, Payload = tarPayload };
    }

    public RegistryOperation BuildRegistryOperation(Guid collectionId, Guid journalId, DateTime submittedAt,
        StixObject stixObject, RegistryOperationType operationType)
    {
        return new RegistryOperation
        {
            CollectionId = collectionId,
            ObjectId = stixObject.Id,
            JournalId = journalId,
            SubmittedAt = submittedAt,
            OperationType = operationType
        };
    }

    public RegistrationReceipt BuildRegistrationReceipt(RegistryOperation operation, RegistryResponse registryResponse,
        ObjectMetadata objectMetadata, StixObject stixObject)
    {
        return new RegistrationReceipt
        {
            JournalId = operation.JournalId,
            CollectionId = operation.CollectionId,
            OperationType = operation.OperationType,
            State = RegistrationReceiptState.Sent,
            ReceiptId = registryResponse.Receipt.ReceiptId,
            ObjectKey = objectMetadata.ObjectKey,
            ObjectId = stixObject.Id,
            ObjectVersion = stixObject.Version(operation.SubmittedAt), // TODO FIXME DANGER duplicate

            SubmittedAt = operation.SubmittedAt,
            SentAt = registryResponse.SentAt,
            Metadata = new VersionMetadata
            {
                Version = registryResponse.Receipt.Version, RegistryChecksum = registryResponse.Receipt.Checksum,
            }
        };
    }

    public StixIngestionResult BuildFailedIngestionResult(StixObject stixObject, string? message)
    {
        return new StixIngestionResult
        {
            StixObject = stixObject,
            Resolution = IngestionResultResolution.Failure,
            ResolutionFailureMessage = message
        };
    }
    public StixIngestionResult BuildAbortedIngestionResult(StixObject stixObject)
    {
        return new StixIngestionResult
        {
            StixObject = stixObject,
            Resolution = IngestionResultResolution.Abort,
        };
    }
}