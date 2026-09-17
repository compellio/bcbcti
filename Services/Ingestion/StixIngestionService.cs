using System.Diagnostics;
using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Models.Stix;
using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services.RegistryApi;

namespace Compellio.Bcbcti.Services.Ingestion;

public class StixIngestionService
{
    private readonly StixObjectRepository _stixRepository;
    private readonly RegistrationReceiptsRepository _receiptsRepository;

    private readonly IRegistryApiClient _registryClient;

    private readonly RegistrationPayloadFactory _registrationPayloadFactory;

    public StixIngestionService(StixObjectRepository stixRepository, RegistrationReceiptsRepository receiptsRepository,
        IRegistryApiClient registryClient)
    {
        _stixRepository = stixRepository;
        _receiptsRepository = receiptsRepository;

        _registryClient = registryClient;

        _registrationPayloadFactory = new RegistrationPayloadFactory();
    }

    public async Task<StixIngestionResult[]> ProcessStixObjects(CollectionOptions collection, Guid journalId,
        DateTime submittedAt, StixObject[] objects, CancellationToken ct = default)
    {
        var results = new StixIngestionResult[objects.Length];

        await Parallel.ForEachAsync(Enumerable.Range(0, objects.Length), new ParallelOptions { CancellationToken = ct },
            async (i, ctoken) =>
            {
                // TODO error handling -> failure ingestion result
                results[i] = await ProcessStixObject(collection, journalId, submittedAt, objects[i], ctoken);
            });

        return results;
    }

    public async Task<StixIngestionResult> ProcessStixObject(CollectionOptions collection, Guid journalId,
        DateTime submittedAt, StixObject stixObject, CancellationToken ct = default)
    {
        // 1. Determine create/update/abort (based on /registrations/{objectId}.json object) directory
        // TODO create = no object exists; update = object exists AND no pending receipt; abort = object exists AND pending receipt
        // TODO IF update, SAVE ETAG!!! -> passed on later

        if (false /* if abort */)
        {
            return new StixIngestionResult { Resolution = IngestionResultResolution.Abort, StixObject = stixObject };
        }

        // TODO determine operation
        var operation = RegistryOperation.Create;

        // 2. Store submitted STIX (canonicalisation handled)
        var storedObject = await _stixRepository.StoreStixObject(stixObject, ct);

        // 3. Build registration payload
        var registrationPayload = _registrationPayloadFactory.BuildRegistrationPayload(storedObject.Metadata);

        // 4. Call the Registry API to register payload
        var sentAt = DateTime.UtcNow; // TODO sentAt belongs to IRegistryApiClient
        var registryReceipt = operation switch
        {
            RegistryOperation.Create => await _registryClient.RegisterTarPayload(registrationPayload.Payload),
            RegistryOperation.Update => throw new NotImplementedException(),
            RegistryOperation.Delete => throw new NotImplementedException(),
            _ => throw new UnreachableException() // TODO Question: is this practice?
        };

        var registrationReceipt = _registrationPayloadFactory.BuildRegistrationReceipt(operation, registryReceipt,
            collection, storedObject.Metadata, journalId, submittedAt, sentAt, stixObject);

        // 5. Store receipt metadata
        var registrationReceiptMetadata = await _receiptsRepository.StoreReceipt(registrationReceipt, ct);

        // 6. Create OR update object registration metadata 
        // TODO
        // TODO IF CREATE If-None-Match: *

        return new StixIngestionResult
        {
            Resolution = IngestionResultResolution.Success,
            // ReceiptId = registryReceipt.ReceiptId,
            // ObjectKey = storedObject.Metadata.ObjectKey,
            // Payload = registrationPayload,
            RegistrationReceipt = registrationReceipt,
            StixObject = stixObject
        };
    }

    // TODO webhook callback
    public async Task ReconcileRegistryApiCallback()
    {
        throw new NotImplementedException();
        //    -. read receipt metadata (with lock)
        //    e. update receipt metadata (with read lock)
        //    f. store manifest (lock)
        //    g. store registration pointer (stores state, current version + registry version, latest receipt)
    }
}