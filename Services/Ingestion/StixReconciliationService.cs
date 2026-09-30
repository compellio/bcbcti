using System.Diagnostics;
using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services.RegistryApi;
using Compellio.Bcbcti.Services.RegistryApi.Models;
using Compellio.Bcbcti.Services.Storage.Exceptions;
using Compellio.Bcbcti.Services.Storage.Models;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.Ingestion;

public class StixReconciliationService
{
    private readonly ILogger<StixReconciliationService> _logger;

    private readonly IOptions<IngestionOptions> _options;
    
    private readonly RegistrationReceiptsRepository _registrationReceiptsRepository;
    private readonly ObjectRegistrationRepository _objectRegistrationRepository;
    private readonly StixObjectRepository _stixObjectRepository;
    private readonly ManifestRepository _manifestRepository;
    private readonly RegistryOperationRepository _registryOperationRepository;

    private readonly IRegistryApiClient _registryClient;

    private readonly RegistryOperationsManager _operationsManager;
    private readonly RegistrationPayloadFactory _registrationPayloadFactory;

    public StixReconciliationService(ILogger<StixReconciliationService> logger, IOptions<IngestionOptions> options,
        RegistrationReceiptsRepository registrationReceiptsRepository, ObjectRegistrationRepository objectRegistrationRepository,
        StixObjectRepository stixObjectRepository, RegistryOperationRepository registryOperationRepository,
        ManifestRepository manifestRepository, RegistryOperationsManager operationsManager, IRegistryApiClient registryClient)
    {
        _logger = logger;
        _options = options; // TODO review dependency

        _registrationReceiptsRepository = registrationReceiptsRepository;
        _objectRegistrationRepository = objectRegistrationRepository;
        _stixObjectRepository = stixObjectRepository;
        _manifestRepository = manifestRepository;
        _registryOperationRepository = registryOperationRepository;

        _registryClient = registryClient;

        _operationsManager = operationsManager;
        _registrationPayloadFactory = new RegistrationPayloadFactory();
    }

    public async Task<StixReconciliationResult> ReconcileOperation(ObjectSummary operationMetadata, CancellationToken ct = default)
    {
        var operation = await _registryOperationRepository.GetRegistryOperationByKey(operationMetadata.ObjectKey, ct);

        if (DateTime.UtcNow - operation.Body.SubmittedAt > _options.Value.PendingOperationTimeout)
        {
            return await Finalize(operation.Body, StixReconciliationResult.TimedOut, ct);
        }

        if (!operation.Body.WasSent)
        {
            // Operation wasn't sent to the Registry API (yet, or due to error)
            return StixReconciliationResult.Skip;
        }

        var registryResponse = await _registryClient.GetTar(operation.Body.ReceiptId.Value, ct);
        var completedAt =
            DateTime.UtcNow; // TODO move to RegistryResponse (= time response ware received, should be a field returned by the API)

        return await Finalize(operation.Body, await ReconcileRegistration(registryResponse.Receipt, completedAt, ct), ct);
    }

    public async Task<StixReconciliationResult> ReconcileRegistryResponse(RegistryResponse registryResponse, DateTime completedAt,
        CancellationToken ct = default)
    {
        if (registryResponse.Receipt.Id is null)
        {
            // Operation hasn't completed (yet, or due to error)
            return StixReconciliationResult.Skip;
        }

        // TODO-FIXME duplicate receipt fetch (in the decision)
        var receipt = await _registrationReceiptsRepository.GetReceipt(registryResponse.Receipt.ReceiptId, ct);
        var operation = await _registryOperationRepository.FindRegistryOperation(receipt.Body.ObjectId, ct);

        if (operation is null)
        {
            // Already reconciled
            return StixReconciliationResult.Skip;
        }

        return await Finalize(operation.Body, await ReconcileRegistration(registryResponse.Receipt, completedAt, ct), ct);
    }

    private async Task<StixReconciliationResult> ReconcileRegistration(TarReceipt tarReceipt, DateTime completedAt,
        CancellationToken ct = default)
    {
        try
        {
            return await ApplyReconciliation(tarReceipt, completedAt, ct);
        }
        catch (PutConditionException)
        {
            return StixReconciliationResult.Abort;
        }
    }

    private async Task<StixReconciliationResult> ApplyReconciliation(TarReceipt tarReceipt, DateTime completedAt, CancellationToken ct = default)
    {
        var decision = await ResolveDecision(tarReceipt, completedAt, ct);

        _logger.LogDebug("Reconciling receipt {ReceiptId} ({DecisionString})", tarReceipt.ReceiptId, decision switch
        {
            StixReconciliationDecision.SkipDecision => "will skip",
            StixReconciliationDecision.ReconcileDecision d => $"will reconcile with {d.TarId}",
            _ => throw new UnreachableException()
        });

        if (decision is not StixReconciliationDecision.ReconcileDecision reconciliation)
        {
            return StixReconciliationResult.Skip;
        }

        var receipt = reconciliation.Receipt;

        // Claims the reconciliation
        await _registrationReceiptsRepository.UpdateReceipt(reconciliation.ETag, receipt, ct);

        var manifest = _registrationPayloadFactory.BuildManifestEntry(reconciliation.TarId, reconciliation.CompletedAt, receipt, tarReceipt);
        var manifestMetadata = await _manifestRepository.PutManifestEntry(manifest, ct);

        var version = _registrationPayloadFactory.BuildVersion(tarReceipt.Version, reconciliation.CompletedAt,
            manifestMetadata.Metadata.ObjectKey, receipt);

        switch (receipt.OperationType)
        {
            case RegistryOperationType.Create:
                await _objectRegistrationRepository.PutObjectRegistration(
                    ObjectRegistration.Create(receipt.CollectionId, receipt.ObjectId, reconciliation.TarId, version), ct);
                break;
            case RegistryOperationType.Update:
                var existingRegistration = await _objectRegistrationRepository.FindObjectRegistration(receipt.ObjectId, ct);

                if (existingRegistration is null)
                    return StixReconciliationResult.Failure($"Update reconciliation for {receipt.ObjectId} has no registration");

                var updatedRegistration = existingRegistration.Body.CurrentVersion?.ReceiptId != receipt.ReceiptId
                    ? existingRegistration.Body.AsUpdated(version)
                    : existingRegistration.Body;

                await _objectRegistrationRepository.PutObjectRegistration(updatedRegistration, ct);

                break;

            case RegistryOperationType.Delete:
                throw new NotImplementedException();
        }

        return StixReconciliationResult.Success(receipt.ObjectId);
    }

    private async Task<StixReconciliationDecision> ResolveDecision(TarReceipt tarReceipt, DateTime completedAtCandidate,
        CancellationToken ct = default)
    {
        var receipt = await _registrationReceiptsRepository.GetReceipt(tarReceipt.ReceiptId, ct);

        if (tarReceipt.Id is null)
            return StixReconciliationDecision.Skip;

        var completedAt = receipt.Body.IsCompleted ? receipt.Body.CompletedAt.Value : completedAtCandidate;
        var updatedReceipt = receipt.Body.AsCompleted(completedAt, tarReceipt.Id);

        return StixReconciliationDecision.Reconcile(receipt.Metadata.ETag, updatedReceipt, completedAt, tarReceipt.Id);
    }

    private async Task<StixReconciliationResult> Finalize(RegistryOperation operation, StixReconciliationResult resolution,
        CancellationToken ct = default)
    {
        switch (resolution)
        {
            case StixReconciliationResult.SkipResolution:
                break;
            
            case StixReconciliationResult.TimedOutResolution: 
                await _operationsManager.Abandon(operation, DateTime.UtcNow, "timed out", ct);
                break;
            
            case StixReconciliationResult.FailureResolution f:
                await _operationsManager.Abandon(operation, DateTime.UtcNow, f.Message ?? "failed", ct);
                break;
            
            default:
                await _operationsManager.Complete(operation, ct);
                break;
        }
        
        return resolution;
    }
}