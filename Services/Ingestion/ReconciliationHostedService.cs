using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services.RegistryApi;
using Compellio.Bcbcti.Services.Storage.Models;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.Ingestion;

// TODO review placement & scope
public class ReconciliationHostedService : BackgroundService
{
    private readonly ILogger<ReconciliationHostedService> _logger;

    private readonly StixReconciliationService _stixReconciliationService;

    private readonly RegistryOperationRepository _registryOperationRepository;
    private readonly IRegistryApiClient _registryApiClient;

    private readonly TimeSpan _refreshFrequency;

    public ReconciliationHostedService(IOptions<BcbctiOptions> options, ILogger<ReconciliationHostedService> logger,
        StixReconciliationService stixReconciliationService, RegistryOperationRepository registryOperationRepository,
        IRegistryApiClient registryApiClient)
    {
        _logger = logger;

        // TODO FIXME validation before parsing?
        _refreshFrequency = TimeSpan.ParseExact(options.Value.ReconciliationFrequency, "c", null);

        _registryOperationRepository = registryOperationRepository;
        _stixReconciliationService = stixReconciliationService;
        _registryApiClient = registryApiClient;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("Reconciliation Service starting");

        using PeriodicTimer timer = new(_refreshFrequency);

        try
        {
            await ReconcilePendingOperations(ct); // TODO FIXME pass ExecuteAsync ct?

            while (await timer.WaitForNextTickAsync(ct))
            {
                await ReconcilePendingOperations(ct); // TODO FIXME pass ExecuteAsync ct?
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Reconciliation Service stopping");
        }
    }

    private async Task ReconcilePendingOperations(CancellationToken ct = default)
    {
        _logger.LogDebug("Scanning for pending operations");
        await foreach (var operation in _registryOperationRepository.ListRegistryOperations()
                           .Objects.WithCancellation(ct))
        {
            await ReconcilePendingOperation(operation, ct);
        }
    }

    private async Task ReconcilePendingOperation(ObjectSummary operationMetadata, CancellationToken ct = default)
    {
        var operation = await _registryOperationRepository.GetRegistryOperation(operationMetadata.ObjectKey, ct);

        if (!operation.Body.WasSent)
        {
            // Operation was not sent, either
            //   - error: receiptId was not written in the RegistryOperation => used RegistryOperation data to investigate
            //   - stale (if operation.SubmittedAt < now + staleTimeSpan): act accordingly (mark failed, etc.)
            // Move this in to StixIngestionService.ReconcileOperation?
            return;
        }

        var tarReceipt = await _registryApiClient.GetTar(operation.Body.ReceiptId.Value, ct);
        var completedAt = DateTime.UtcNow; // TODO FIXME move to IRegistryApiClient

        var resolution = await _stixReconciliationService.ReconcileTarReceipt(tarReceipt.Receipt, completedAt, ct);

        if (resolution is StixReconciliationResult.SuccessResolution)
        {
            // delete pending task
            await _registryOperationRepository.DeleteRegistryOperation(operationMetadata.ObjectKey, ct);
        }
    }
}