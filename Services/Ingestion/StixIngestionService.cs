using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bcbcti.Models.Documents;
using Bcbcti.Models.Stix;
using Bcbcti.Repositories;
using Bcbcti.Services.Ingestion.Registry;
using Bcbcti.Services.RegistryApi.Models;
using Bcbcti.Services.Storage.Models;

namespace Bcbcti.Services.Ingestion;

public class StixIngestionService
{
    private readonly StixObjectRepository _stixRepository;
    private readonly RegistrationReceiptsRepository _receiptsRepository;


    public StixIngestionService(StixObjectRepository stixRepository,
        RegistrationReceiptsRepository receiptsRepository /* + registry api client */)
    {
        _stixRepository = stixRepository;
        _receiptsRepository = receiptsRepository;
    }

    public async Task<StixIngestionResult[]> ProcessStixObjects(StixObject[] objects, DateTime submittedAt,
        CancellationToken ct = default)
    {
        var results = new StixIngestionResult[objects.Length];

        await Parallel.ForEachAsync(Enumerable.Range(0, objects.Length),
            new ParallelOptions { CancellationToken = ct },
            async (i, ctoken) => { results[i] = await ProcessStixObject(objects[i], submittedAt, ctoken); });

        return results;
    }

    // TODO error handling
    public async Task<StixIngestionResult> ProcessStixObject(StixObject resource, DateTime submittedAt,
        CancellationToken ct = default)
    {
        // 1. Determine create/update/abort (based on /registrations/{objectId}.json object) directory
        // TODO create = no object exists; update = object exists AND no pending receipt; abort = object exists AND pending receipt

        // 2. Store submitted STIX (canonicalisation handled)
        var storedObject = await _stixRepository.StoreStixObject(resource, ct);

        // 3. Build registration payload
        var stixArtifact = BuildStixArtifact(storedObject.Metadata);
        var stixBundle = BuildStixBundle([stixArtifact]);
        var tarPayload = BuildTarPayload(stixBundle);

        // 4. Call the Registry API to register payload
        var sentAt = DateTime.UtcNow;
        var registryReceipt = await RegisterTarPayload(tarPayload);

        // 5. Store receipt metadata
        var registrationReceipt = new RegistrationReceipt
        {
            State = RegistrationReceiptState.Sent,
            ReceiptId = registryReceipt.ReceiptId,
            ObjectKey = storedObject.Metadata.ObjectKey,
            SubmittedAt = submittedAt,
            SentAt = sentAt,
            Metadata = new RegistrationMetadata
            {
                Version = registryReceipt.Version,
                RegistryChecksum = registryReceipt.Checksum,
                
                ObjectBundleId = stixBundle.Id,
                ObjectArtifactId = stixArtifact.Id,
            }
        };
        
        var registrationReceiptMetadata = await _receiptsRepository.StoreReceipt(registrationReceipt, ct);

        // 6. Store object registration metadata
        // TODO

        return new StixIngestionResult
        {
            ReceiptId = registryReceipt.ReceiptId,
            ObjectKey = storedObject.Metadata.ObjectKey,
            TarPayload = tarPayload
        };
    }

    // TODO webhook callback
    public async Task ReconcileRegistryApiCallback()
    {
        //    -. read receipt metadata (with lock)
        //    e. update receipt metadata (with read lock)
        //    f. store manifest (lock)
        //    g. store registration pointer (stores state, current version + registry version, latest receipt)
    }

    private TarPayload BuildTarPayload(StixBundle bundle)
    {
        return new TarPayload
        {
            JsonLdContext = "urn:tar:...", // TODO fixme location (but, should be hardcoded)
            JsonLdType = "stixBundle", // TODO fixme location
            Bundle = bundle
        };
    }

    private StixBundle BuildStixBundle(StixObject[] objects)
    {
        var bundleGuid = Guid.NewGuid();

        return new StixBundle(bundleGuid)
        {
            Objects = objects
        };
    }

    private StixArtifact BuildStixArtifact(ObjectMetadata metadata)
    {
        var artifactGuid = Guid.NewGuid(); // TODO uuidv5 based on result.Metadata.PublicObjectUrl

        return new StixUrlArtifact(artifactGuid)
        {
            Url = metadata.PublicObjectUrl,
            MimeType = "application/stix+json;version=2.1",
            Hashes = new Dictionary<StixHashAlgorithm, string>
            {
                { StixHashAlgorithm.SHA_256, metadata.ChecksumSha256 }
            }
        };
    }

    // TODO Registry API call
    private async Task<TarReceipt> RegisterTarPayload(TarPayload tarPayload)
    {
        // TODO somehow globally configure
        // see BCBCTI.Services.Serialization.Converters.StixJsonConverter
        var stixSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        stixSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));

        // TODO WARNING!!! ONLY THE INNER STIX BUNDLE NEEDS TO BE SERIALIZED WITH stixSerializerOptions
        //                 THE REMAINING TAR ENVELOPE SHOULD NOT (snake case, etc.)!
        var data = JsonSerializer.Serialize(tarPayload, stixSerializerOptions);

        Console.WriteLine($"TODO Call Registry API to register: {data}");
        // dummy response (!careful: checksum in hex, not base64)

        return new TarReceipt
        {
            ReceiptId = Guid.NewGuid(),
            Checksum = "0x7A38BF81F383F69433AD6E900D35B3E2385593F76A7B7AB5D4355B8BA41EE24B",
            Version = 0,
            Data = JsonDocument.Parse(data)
        };
    }
}