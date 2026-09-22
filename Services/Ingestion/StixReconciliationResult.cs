namespace Compellio.Bcbcti.Services.Ingestion;

public abstract record StixReconciliationResult
{
    private protected StixReconciliationResult()
    {
    }

    public sealed record SuccessResolution : StixReconciliationResult
    {
        public required string ObjectId { get; init; }
    };

    public sealed record AbortResolution : StixReconciliationResult;

    public sealed record FailureResolution : StixReconciliationResult;

    public sealed record SkipResolution : StixReconciliationResult;

    public static StixReconciliationResult Success(string objectId) => new SuccessResolution
    {
        ObjectId = objectId
    };

    public static readonly StixReconciliationResult Abort = new AbortResolution();
    public static readonly StixReconciliationResult Failure = new FailureResolution();
    public static readonly StixReconciliationResult Skip = new SkipResolution();
}