using System.Diagnostics;
using Compellio.Bcbcti.Extensions;
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

    public async Task<IReadOnlyList<StixIngestionResult>> ProcessStixObjects(CollectionOptions collection,
        Guid journalId,
        DateTime submittedAt, StixObject[] objects, CancellationToken ct = default)
    {
        // TODO set ParallelOptions.MaxDegreeOfParallelism based on project restrictions
        //   (e.g. AmazonS3Config.MaxConnectionsPerServer w/ default = 50, Registry API rate limits)
        return await objects.ParallelSelectAsync(async (stixObject, ctoken) =>
        {
            try
            {
                return await ProcessStixObject(collection, journalId, submittedAt, stixObject, ctoken);
            }
            catch (PutConditionException)
            {
                return _registrationPayloadFactory.BuildFailedIngestionResult(stixObject, "duplicate object");
            }
            catch (ProviderOperationException e)
            {
                _logger.LogError(e, "Error during read/write operations");
                return
                    _registrationPayloadFactory.BuildFailedIngestionResult(stixObject,
                        "Error during read/write operations");
            }
            catch (InvalidDataException) // TODO FIXME imprecise exception (used in JsonObjectStore)
            {
                return
                    _registrationPayloadFactory.BuildFailedIngestionResult(stixObject, "Error processing object");
            }
        }, ct);
    }

    public async Task<StixIngestionResult> ProcessStixObject(CollectionOptions collection, Guid journalId,
        DateTime submittedAt, StixObject stixObject, CancellationToken ct = default)
    {
        // 1. Determine create/update/abort
        var decision = await ResolveDecision(stixObject, ct);

        _logger.LogDebug("Ingesting object {ObjectId} ({DecisionString})", stixObject.Id, decision switch
        {
            StixIngestionDecision.AbortDecision => "will abort",
            StixIngestionDecision.CreateDecision => "will create",
            StixIngestionDecision.UpdateDecision d => $"will update {d.TarId}",
            _ => "unknown"
        });

        if (decision is StixIngestionDecision.AbortDecision)
        {
            return _registrationPayloadFactory.BuildAbortedIngestionResult(stixObject);
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