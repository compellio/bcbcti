using System.Diagnostics;
using Amazon.S3.Transfer;
using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Models.Stix;
using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services.RegistryApi;
using Compellio.Bcbcti.Services.Storage.Exceptions;

namespace Compellio.Bcbcti.Services.Ingestion;

public class StixIngestionService
{
    private readonly StixObjectRepository _stixRepository;
    private readonly RegistrationReceiptsRepository _receiptsRepository;
    private readonly ObjectRegistrationRepository _objectRegistrationRepository;

    private readonly IRegistryApiClient _registryClient;

    private readonly RegistrationPayloadFactory _registrationPayloadFactory;

    public StixIngestionService(StixObjectRepository stixRepository, RegistrationReceiptsRepository receiptsRepository,
        ObjectRegistrationRepository objectRegistrationRepository, IRegistryApiClient registryClient)
    {
        _stixRepository = stixRepository;
        _receiptsRepository = receiptsRepository;
        _objectRegistrationRepository = objectRegistrationRepository;

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
                try
                {
                    results[i] = await ProcessStixObject(collection, journalId, submittedAt, objects[i], ctoken);
                }
                catch (ProviderOperationException)
                {
                    results[i] = BuildFailedIngestionResult(objects[i], "Error during read/write operations");
                }
                catch (InvalidDataException) // TODO FIXME imprecise exception (used in JsonObjectStore)
                {
                    results[i] = BuildFailedIngestionResult(objects[i], "Error processing object");
                }
            });

        return results;
    }

    public async Task<StixIngestionResult> ProcessStixObject(CollectionOptions collection, Guid journalId,
        DateTime submittedAt, StixObject stixObject, CancellationToken ct = default)
    {
        // 1. Determine create/update/abort
        var decision = await ResolveDecision(stixObject, ct);

        if (decision is StixIngestionDecision.AbortDecision)
        {
            return new StixIngestionResult { Resolution = IngestionResultResolution.Abort, StixObject = stixObject };
        }

        // 2. Store submitted STIX (canonicalisation handled)
        var storedObject = await _stixRepository.StoreStixObject(stixObject, ct);

        // 3. Build registration payload
        var registrationPayload = _registrationPayloadFactory.BuildRegistrationPayload(storedObject.Metadata);

        // 4. Call the Registry API to register payload
        var registryReceipt = decision switch
        {
            StixIngestionDecision.CreateDecision => await _registryClient.RegisterTarPayload(
                registrationPayload.Payload),
            StixIngestionDecision.UpdateDecision d => await _registryClient.UpdateTarPayload(d.TarId,
                registrationPayload.Payload),
            _ => throw new UnreachableException()
        };

        // TODO DANGER RACE CONDITION: what if registry calls back the webhook before we have time to write the receipt?
        //     Future solution: custom user-defined identification key sent to Registry that Registry includes in webhooks, etc. => allows for caller to setup webhook handling before calling registry API  

        var operation = decision switch
        {
            StixIngestionDecision.CreateDecision => RegistryOperation.Create,
            StixIngestionDecision.UpdateDecision => RegistryOperation.Update,
            _ => throw new UnreachableException()
        };

        var registrationReceipt = _registrationPayloadFactory.BuildRegistrationReceipt(operation, registryReceipt,
            collection, storedObject.Metadata, journalId, submittedAt, stixObject);

        // 5. Store receipt metadata
        var registrationReceiptMetadata = await _receiptsRepository.StoreReceipt(registrationReceipt, ct);

        // 6. Create or update object registration metadata
        var objectRegistrationMetadata = decision switch
        {
            StixIngestionDecision.CreateDecision => await _objectRegistrationRepository.StoreObjectRegistration(
                ObjectRegistration.Create(stixObject, collection.Id), ct),
            StixIngestionDecision.UpdateDecision d => await _objectRegistrationRepository.PutObjectRegistration(d.ETag,
                d.ObjectRegistration.AsUpdating(), ct),
            _ => throw new UnreachableException()
        };

        return new StixIngestionResult
        {
            Resolution = IngestionResultResolution.Success,
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

    private async Task<StixIngestionDecision> ResolveDecision(StixObject stixObject, CancellationToken ct = default)
    {
        var objectRegistration = await _objectRegistrationRepository.FindObjectRegistration(stixObject.Id, ct);

        if (objectRegistration is null)
        {
            // create = no registered object exists
            return StixIngestionDecision.Create;
        }

        if (objectRegistration.Body.IsRegistered)
        {
            return StixIngestionDecision.Update(objectRegistration.Metadata.ETag,
                objectRegistration.Body,
                objectRegistration.Body.TarId);
        }

        if (objectRegistration.Body.IsMutating)
        {
            return StixIngestionDecision.Abort;
        }
        
        throw new UnreachableException();
    }

    private StixIngestionResult BuildFailedIngestionResult(StixObject stixObject, string? message)
    {
        return new StixIngestionResult
        {
            StixObject = stixObject,
            Resolution = IngestionResultResolution.Failure,
            ResolutionFailureMessage = message
        };
    }
}