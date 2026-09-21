using System.Diagnostics;
using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services.RegistryApi.Models;

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

    public async Task<StixReconciliationResult> ReconcileTarReceipt(TarReceipt tarReceipt, DateTime completedAt, CancellationToken ct)
    {
        if (tarReceipt.Id is null)
        {
            return StixReconciliationResult.Skip;
        }

        var receipt = await _registrationReceiptsRepository.GetReceipt(tarReceipt.ReceiptId, ct);
        var registration = await _objectRegistrationRepository.GetObjectRegistration(receipt.Body.ObjectId, ct);

        var manifest = new ManifestEntry
        {
            CompletedAt = completedAt,
            ObjectId = receipt.Body.ObjectId,
            ObjectKey = receipt.Body.ObjectKey,
            ReceiptId = receipt.Body.ReceiptId,
            VersionMetadata = new RegistrationMetadata()
            {
                RegistryChecksum = tarReceipt.Checksum,
                TarId = tarReceipt.Id,
                Version = tarReceipt.Version,
            }
        };
        
        var manifestMetadata = await _manifestRepository.CreateManifestEntry(manifest, ct);

        var version = new ObjectRegistration.Version()
        {
            ReceiptId = receipt.Body.ReceiptId,
            ManifestKey = manifestMetadata.Metadata.ObjectKey,
            ObjectKey = receipt.Body.ObjectKey,
            
            TarVersion = tarReceipt.Version,
            ObjectVersion = receipt.Body.ObjectVersion,
            
            CompletedAt = completedAt,
        };

        var updatedReceipt = receipt.Body.AsCompleted(completedAt, tarReceipt.Id);

        var updatedRegistration = updatedReceipt.OperationType switch
        {
            RegistryOperationType.Create => registration.Body.AsCreated(tarReceipt.Id, version),
            RegistryOperationType.Update => registration.Body.AsUpdated(version),
            RegistryOperationType.Delete => throw new NotImplementedException(),
            _ => throw new UnreachableException()
        };

        await _registrationReceiptsRepository.UpdateReceipt(receipt.Metadata.ETag, updatedReceipt, ct);
        await _objectRegistrationRepository.UpdateObjectRegistration(registration.Metadata.ETag, updatedRegistration, ct);

        return StixReconciliationResult.Success;
    }

}