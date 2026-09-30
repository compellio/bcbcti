// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

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

    private readonly RegistryOperationsManager _operationsManager;
    private readonly RegistrationPayloadFactory _registrationPayloadFactory;

    public StixIngestionService(ILogger<StixIngestionService> logger, StixObjectRepository stixRepository,
        RegistrationReceiptsRepository receiptsRepository,
        ObjectRegistrationRepository objectRegistrationRepository,
        RegistryOperationRepository registryOperationRepository, RegistryOperationsManager operationsManager, IRegistryApiClient registryClient
    )
    {
        _logger = logger;

        _stixRepository = stixRepository;
        _receiptsRepository = receiptsRepository;
        _objectRegistrationRepository = objectRegistrationRepository;
        _registryOperationRepository = registryOperationRepository;

        _registryClient = registryClient;

        _operationsManager = operationsManager;
        _registrationPayloadFactory = new RegistrationPayloadFactory();
    }

    public async Task<IReadOnlyList<StixIngestionResult>> ProcessStixObjects(CollectionOptions collection,
        Guid journalId, DateTime submittedAt, StixObject[] objects, CancellationToken ct = default)
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
                return _registrationPayloadFactory.BuildFailedIngestionResult(stixObject,
                    "Error during read/write operations");
            }
            catch (InvalidDataException) // TODO FIXME imprecise exception (used in JsonObjectStore)
            {
                return _registrationPayloadFactory.BuildFailedIngestionResult(stixObject, "Error processing object");
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

        // 2. Register operation -- TODO move to RegistryOperationsManager at some point
        var operation = _registrationPayloadFactory.BuildRegistryOperation(collection.Id, journalId, submittedAt,
            stixObject, decision switch
            {
                StixIngestionDecision.CreateDecision => RegistryOperationType.Create,
                StixIngestionDecision.UpdateDecision => RegistryOperationType.Update,
                _ => throw new UnreachableException()
            });
        var operationMetadata = await _registryOperationRepository.CreateRegistryOperation(operation, ct);

        try
        {
            // 3. Store submitted STIX (canonicalization handled)
            var storedObject = await _stixRepository.StoreStixObject(stixObject, ct);

            // 3.b. Attach submitted STIX to the operation -- TODO move to RegistryOperationsManager at some point
            // Note: a cleaner design would be to calculate the objectKey before writing storedObject (-1 store update), but
            //   this is something that currently lives behind the PutContentAddressedObjectAsync method. Review when possible.
            operation = operation.WithObjectKey(storedObject.Metadata.ObjectKey);
            operationMetadata = await _registryOperationRepository.UpdateRegistryOperation(operationMetadata.Metadata.ETag, operation, ct);

            // 4. Prepare registry payload and operation
            var registrationPayload = _registrationPayloadFactory.BuildRegistrationPayload(storedObject.Metadata);

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
            var registrationReceipt = _registrationPayloadFactory.BuildRegistrationReceipt(operation, registryResponse,
                storedObject.Metadata, stixObject);
            await _receiptsRepository.StoreReceipt(registrationReceipt, ct);

            // 8. Update operation -- TODO move to RegistryOperationsManager at some point
            operation = operation.WithReceipt(registrationReceipt.ReceiptId);
            await _registryOperationRepository.UpdateRegistryOperation(operationMetadata.Metadata.ETag, operation, ct);

            return new StixIngestionResult
            {
                Resolution = IngestionResultResolution.Success,
                RegistrationReceipt = registrationReceipt,
                StixObject = stixObject
            };
        }
        catch (Exception e)
        {
            await _operationsManager.Abandon(operation, DateTime.UtcNow, "failed", ct);
            throw;
        }
    }

    private async Task<StixIngestionDecision> ResolveDecision(StixObject stixObject, CancellationToken ct = default)
    {
        var operation = await _registryOperationRepository.FindRegistryOperation(stixObject.Id, ct);

        if (operation is not null)
        {
            return StixIngestionDecision.Abort;
        }

        var objectRegistration = await _objectRegistrationRepository.FindObjectRegistration(stixObject.Id, ct);

        if (objectRegistration is null)
        {
            return StixIngestionDecision.Create;
        }

        return StixIngestionDecision.Update(objectRegistration.Metadata.ETag, objectRegistration.Body,
            objectRegistration.Body.TarId);
    }
}
