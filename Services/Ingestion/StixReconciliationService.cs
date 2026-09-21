using System.Diagnostics;
using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services.RegistryApi.Models;
using Compellio.Bcbcti.Services.Storage.Exceptions;

namespace Compellio.Bcbcti.Services.Ingestion;

public class StixReconciliationService
{
    private readonly ILogger<StixReconciliationService> _logger;

    private readonly RegistrationReceiptsRepository _registrationReceiptsRepository;
    private readonly ObjectRegistrationRepository _objectRegistrationRepository;
    private readonly ManifestRepository _manifestRepository;

    public StixReconciliationService(ILogger<StixReconciliationService> logger,
        RegistrationReceiptsRepository registrationReceiptsRepository,
        ObjectRegistrationRepository objectRegistrationRepository, ManifestRepository manifestRepository)
    {
        _logger = logger;

        _registrationReceiptsRepository = registrationReceiptsRepository;
        _objectRegistrationRepository = objectRegistrationRepository;
        _manifestRepository = manifestRepository;
    }

    public async Task<StixReconciliationResult> ReconcileTarReceipt(TarReceipt tarReceipt,
        DateTime completedAtCandidate, CancellationToken ct)
    {
        var decision = await ResolveDecision(tarReceipt, completedAtCandidate, ct);
        
        _logger.LogDebug("Reconciling receipt {ReceiptId} ({DecisionString})", tarReceipt.ReceiptId, decision switch
        {
            StixReconciliationDecision.SkipDecision => "will skip",
            StixReconciliationDecision.UpdateDecision d => $"will update {d.TarId}",
            _ => "unknown"
        });

        if (decision is not StixReconciliationDecision.UpdateDecision update)
        {
            return StixReconciliationResult.Skip;
        }
        
        var receipt = update.Receipt;
        var registration = await _objectRegistrationRepository.GetObjectRegistration(receipt.ObjectId, ct);

        var manifest = new ManifestEntry
        {
            CompletedAt = update.CompletedAt,
            ObjectId = receipt.ObjectId,
            ObjectKey = receipt.ObjectKey,
            ReceiptId = receipt.ReceiptId,
            RegistrationMetadata = new RegistrationMetadata()
            {
                RegistryChecksum = tarReceipt.Checksum,
                TarId = update.TarId,
                Version = tarReceipt.Version,
            }
        };
        
        try
        {
            // Claims the reconciliation
            await _registrationReceiptsRepository.UpdateReceipt(update.ETag, receipt, ct);
        }
        catch (PutConditionException)
        {
            // Race condition conflict
            // Review design (using exceptions for control)
            return StixReconciliationResult.Abort;
        }

        var manifestMetadata = await _manifestRepository.PutManifestEntry(manifest, ct);

        var version = new ObjectRegistration.Version
        {
            ReceiptId = receipt.ReceiptId,
            ManifestKey = manifestMetadata.Metadata.ObjectKey,
            ObjectKey = receipt.ObjectKey,

            TarVersion = tarReceipt.Version,
            ObjectVersion = receipt.ObjectVersion,

            CompletedAt = update.CompletedAt,
        };
        
        if (registration.Body.CurrentVersion?.ReceiptId != receipt.ReceiptId)
        {
            var updatedRegistration = receipt.OperationType switch
            {
                RegistryOperationType.Create => registration.Body.AsCreated(update.TarId, version),
                RegistryOperationType.Update => registration.Body.AsUpdated(version),
                RegistryOperationType.Delete => throw new NotImplementedException(),
                _ => throw new UnreachableException()
            };
        
            await _objectRegistrationRepository.UpdateObjectRegistration(registration.Metadata.ETag,
                updatedRegistration, ct);
        }

        return StixReconciliationResult.Success(receipt.ObjectId);
    }

    private async Task<StixReconciliationDecision> ResolveDecision(TarReceipt tarReceipt, DateTime completedAtCandidate,
        CancellationToken ct = default)
    {
        var receipt = await _registrationReceiptsRepository.GetReceipt(tarReceipt.ReceiptId, ct);

        if (tarReceipt.Id is null)
        {
            return StixReconciliationDecision.Skip;
        }

        var completedAt = receipt.Body.IsCompleted ? receipt.Body.CompletedAt.Value : completedAtCandidate;

        var updatedReceipt = receipt.Body.AsCompleted(completedAt, tarReceipt.Id);

        return StixReconciliationDecision.Update(receipt.Metadata.ETag, updatedReceipt, completedAt, tarReceipt.Id);
    }
}