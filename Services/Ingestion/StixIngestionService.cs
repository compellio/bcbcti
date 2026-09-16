using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bcbcti.Models.Registry;
using Bcbcti.Models.Stix;
using Bcbcti.Models.Taxii;
using BCBCTI.Models.Taxii;
using Bcbcti.Repositories;
using Bcbcti.Services.Ingestion.Registry;

namespace Bcbcti.Services.Ingestion;

public class StixIngestionService
{
    private readonly StixObjectRepository _stixRepository;

    public StixIngestionService(StixObjectRepository stixRepository /* + registry api client */)
    {
        _stixRepository = stixRepository;
    }

    public async Task<StixIngestionResult[]> ProcessStixObjects(StixObject[] objects, DateTime submittedAt,
        CancellationToken ct = default)
    {
        var results = new StixIngestionResult[objects.Length];
        
        await Parallel.ForEachAsync(Enumerable.Range(0, objects.Length),
            new ParallelOptions { CancellationToken = ct },
            async (i, ctoken) =>
            {
                results[i] = await ProcessStixObject(objects[i], submittedAt, ctoken);
            });

        return results;
    }

    // TODO error handling
    public async Task<StixIngestionResult> ProcessStixObject(StixObject resource, DateTime submittedAt,
        CancellationToken ct = default)
    {
        // 1. Determine create/update/abort (based on /registrations/{objectId}.json object) directory
        // TODO create = no object exists; update = object exists AND no pending receipt; abort = object exists AND pending receipt

        // 2. Store submitted STIX (canonicalisation handled)
        var result = await _stixRepository.PutStixObject(resource, ct);

        var bundleGuid = Guid.NewGuid();
        var artifactGuid = Guid.NewGuid(); // TODO uuidv5 based on URI

        // 3. Build registration payload
        var tarPayload = new TarPayload
        {
            JsonLdContext = "urn:tar:...", // TODO fixme location (but, should be hardcoded)
            JsonLdType = "stixBundle", // TODO fixme location
            Bundle = new StixBundle(bundleGuid)
            {
                Objects =
                [
                    new StixUrlArtifact(artifactGuid)
                    {
                        Url = result.Metadata.PublicObjectUrl,
                        MimeType = "application/stix+json;version=2.1",
                        Hashes = new Dictionary<StixHashAlgorithm, string>
                        {
                            { StixHashAlgorithm.SHA_256, result.Metadata.ChecksumSha256 }
                        }
                    }
                ]
            }
        };

        // 4. Call the Registry API to register payload
        // TODO === REGISTRY CALL ===
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
        Console.WriteLine(
            $"TODO Call Registry API to register: {data}");
        // dummy response (!careful: checksum in hex, not base64)
        var receipt = new ReceiptResource
        {
            ReceiptId = Guid.NewGuid(),
            Checksum = "0x7A38BF81F383F69433AD6E900D35B3E2385593F76A7B7AB5D4355B8BA41EE24B",
            Version = 0,
            Data = JsonDocument.Parse(data)
        };
        // TODO === REGISTRY CALL ===

        // 5. Store receipt metadata (with lock)
        // TODO

        // 6. Store object registration metadata (with lock)
        // TODO

        //    --- webhook callback ---
        //    e. store manifest (lock)
        //    f. store registration pointer (stores state, current version + registry version, latest receipt)

        return new StixIngestionResult
        {
            ReceiptId = receipt.ReceiptId,
            ObjectKey = result.Metadata.ObjectKey,
        };
    }
}