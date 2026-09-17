using Bcbcti.Models.Documents;
using Bcbcti.Models.Stix;
using Bcbcti.Options;
using Bcbcti.Services.Ingestion.Registry;
using Bcbcti.Services.RegistryApi.Models;
using Bcbcti.Services.RegistryApi.Profiles;
using Bcbcti.Services.Storage.Models;

namespace Bcbcti.Services.Ingestion;

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
        // TODO uuidv5 based on result.Metadata.PublicObjectUrl
        // TODO FIXME Stix objects should have factories -> the UUID calculation is something that could/should be encapsulated

        var artifactGuid = Guid.NewGuid();

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
        Guid journalId, DateTime submittedAt, DateTime sentAt, StixObject stixObject)
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
            SentAt = sentAt,

            Metadata = new RegistrationMetadata
            {
                Version = receipt.Version,
                RegistryChecksum = receipt.Checksum,
            }
        };
    }
}