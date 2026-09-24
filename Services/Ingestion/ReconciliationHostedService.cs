using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Repositories;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.Ingestion;

// TODO-REVIEW placement & scope
public class ReconciliationHostedService : BackgroundService
{
    private readonly ILogger<ReconciliationHostedService> _logger;

    private readonly IOptions<IngestionOptions> _options;

    private readonly StixReconciliationService _stixReconciliationService;

    private readonly RegistryOperationRepository _registryOperationRepository;

    public ReconciliationHostedService(IOptions<IngestionOptions> options, ILogger<ReconciliationHostedService> logger,
        StixReconciliationService stixReconciliationService, RegistryOperationRepository registryOperationRepository)
    {
        _logger = logger;

        _options = options;

        _registryOperationRepository = registryOperationRepository;
        _stixReconciliationService = stixReconciliationService;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        _logger.LogInformation("Ingestion Service starting");

        using var timer = new PeriodicTimer(_options.Value.ReconciliationFrequency);

        try
        {
            await ReconcilePendingOperations(ct);

            while (await timer.WaitForNextTickAsync(ct))
            {
                await ReconcilePendingOperations(ct);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Ingestion Service stopping");
        }
    }

    private async Task ReconcilePendingOperations(CancellationToken ct = default)
    {
        _logger.LogDebug("Scanning for pending operations");
        await foreach (var operation in _registryOperationRepository.ListRegistryOperations()
                           .Objects.WithCancellation(ct))
        {
            try
            {
                await _stixReconciliationService.ReconcileOperation(operation, ct);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error reconciling pending operation {Key}; will retry", operation.ObjectKey);
            }
        }
    }
}