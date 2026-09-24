namespace Compellio.Bcbcti.Options;

public class IngestionOptions
{
    public TimeSpan ReconciliationFrequency { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan PendingOperationTimeout { get; set; } = TimeSpan.FromHours(4);
}