using System.Diagnostics;
using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Models.Stix;
using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services.RegistryApi;
using Compellio.Bcbcti.Services.Storage.Exceptions;

namespace Compellio.Bcbcti.Services.Ingestion;

public class StixIngestionService
{
    private readonly ILogger<StixIngestionService> _logger;

    private readonly StixObjectRepository _stixRepository;
    private readonly RegistrationReceiptsRepository _receiptsRepository;
    private readonly ObjectRegistrationRepository _objectRegistrationRepository;
    private readonly RegistryOperationRepository _registryOperationRepository;

    private readonly IRegistryApiClient _registryClient;

    private readonly RegistrationPayloadFactory _registrationPayloadFactory;

    public StixIngestionService(StixObjectRepository stixRepository, RegistrationReceiptsRepository receiptsRepository,
        ObjectRegistrationRepository objectRegistrationRepository,
        RegistryOperationRepository registryOperationRepository, IRegistryApiClient registryClient,
        ILogger<StixIngestionService> logger)
    {
        _logger = logger;

        _stixRepository = stixRepository;
        _receiptsRepository = receiptsRepository;
        _objectRegistrationRepository = objectRegistrationRepository;
        _registryOperationRepository = registryOperationRepository;

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
                    results[i] =
                        _registrationPayloadFactory.BuildFailedIngestionResult(objects[i],
                            "Error during read/write operations");
                }
                catch (InvalidDataException) // TODO FIXME imprecise exception (used in JsonObjectStore)
                {
                    results[i] =
                        _registrationPayloadFactory.BuildFailedIngestionResult(objects[i], "Error processing object");
                }
            });

        return results;
    }

    public async Task<StixIngestionResult> ProcessStixObject(CollectionOptions collection, Guid journalId,
        DateTime submittedAt, StixObject stixObject, CancellationToken ct = default)
    {
        // 1. Determine create/update/abort
        var decision = await ResolveDecision(stixObject, ct);

        _logger.LogDebug("Ingesting {ObjectId} ({DecisionString})", stixObject.Id, decision switch
        {
            StixIngestionDecision.AbortDecision => "will abort",
            StixIngestionDecision.CreateDecision => "will create",
            StixIngestionDecision.UpdateDecision d => $"will update {d.TarId}",
            _ => "unknown"
        });

        if (decision is StixIngestionDecision.AbortDecision)
        {
            return new StixIngestionResult { Resolution = IngestionResultResolution.Abort, StixObject = stixObject };
        }

        // 2. Store submitted STIX (canonicalization handled)
        var storedObject = await _stixRepository.StoreStixObject(stixObject, ct);

        // 3. Prepare registry payload and operation
        var registrationPayload = _registrationPayloadFactory.BuildRegistrationPayload(storedObject.Metadata);

        // 4. Register operation
        var operation =
            await StoreRegistrationOperation(collection.Id, journalId, submittedAt, decision, stixObject, ct);

        var operationMetadata = await _registryOperationRepository.CreateRegistryOperation(operation, ct);

        // 5. Call the Registry API to register payload
        var registryResponse = decision switch
        {
            StixIngestionDecision.CreateDecision => await _registryClient.RegisterTarPayload(
                registrationPayload.Payload, ct),
            StixIngestionDecision.UpdateDecision d => await _registryClient.UpdateTarPayload(d.TarId,
                registrationPayload.Payload, ct),
            _ => throw new UnreachableException()
        };

        // 6. Store receipt metadata
        var registrationReceipt =
            _registrationPayloadFactory.BuildRegistrationReceipt(operation, registryResponse, storedObject.Metadata,
                stixObject);
        var registrationReceiptMetadata = await _receiptsRepository.StoreReceipt(registrationReceipt, ct);

        // 7. Update operation
        await _registryOperationRepository.UpdateRegistryOperation(operationMetadata.Metadata.ETag,
            operation.WithReceipt(registrationReceipt.ReceiptId), ct);

        return new StixIngestionResult
        {
            Resolution = IngestionResultResolution.Success,
            RegistrationReceipt = registrationReceipt,
            StixObject = stixObject
        };
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
            return StixIngestionDecision.Update(objectRegistration.Metadata.ETag, objectRegistration.Body,
                objectRegistration.Body.TarId);
        }

        if (objectRegistration.Body.IsPending)
        {
            return StixIngestionDecision.Abort;
        }

        throw new UnreachableException();
    }

    /// <summary>
    /// Stores the intent to of a new registration operation (creates a pending operation)
    /// TODO FIXME StoreRegistrationOperation has partial side effect: _registryOperationRepository.CreateRegistryOperation called from outside
    /// </summary>
    private async Task<RegistryOperation> StoreRegistrationOperation(Guid collectionId, Guid journalId,
        DateTime submittedAt, StixIngestionDecision decision, StixObject stixObject, CancellationToken ct = default)
    {
        switch (decision)
        {
            case StixIngestionDecision.CreateDecision:
                var createOperation = _registrationPayloadFactory.BuildRegistryOperation(collectionId, journalId,
                    submittedAt, stixObject, RegistryOperationType.Create);
                await _objectRegistrationRepository.CreateObjectRegistration(
                    ObjectRegistration.Create(stixObject, collectionId), ct);
                return createOperation;

            case StixIngestionDecision.UpdateDecision d:
                var updateOperation = _registrationPayloadFactory.BuildRegistryOperation(collectionId, journalId,
                    submittedAt, stixObject, RegistryOperationType.Update);
                await _objectRegistrationRepository.UpdateObjectRegistration(d.ETag, d.ObjectRegistration.AsUpdating(),
                    ct);
                return updateOperation;

            default:
                throw new UnreachableException();
        }
    }
}