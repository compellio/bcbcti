using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Models.Stix;
using Compellio.Bcbcti.Options;
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

        return new RegistrationPayload
        {
            Artifact = stixArtifact,
            Bundle = stixBundle,
            Payload = tarPayload
        };
    }

    public RegistrationReceipt BuildRegistrationReceipt(RegistryOperation operation, TarReceipt receipt, CollectionOptions collection, ObjectMetadata objectMetadata,
        Guid journalId, DateTime submittedAt, StixObject stixObject)
    {
        return new RegistrationReceipt
        {
            JournalId = journalId,
            CollectionId = collection.Id,
            
            Operation = operation,
            
            State = RegistrationReceiptState.Sent,
            ReceiptId = receipt.ReceiptId,
            
            ObjectKey = objectMetadata.ObjectKey,
            ObjectId = stixObject.Id,
            ObjectVersion = stixObject.Version(submittedAt), // TODO FIXME DANGER duplicate

            SubmittedAt = submittedAt,
            SentAt = receipt.SentAt,

            Metadata = new RegistrationMetadata
            {
                Version = receipt.Version,
                RegistryChecksum = receipt.Checksum,
            }
        };
    }
}