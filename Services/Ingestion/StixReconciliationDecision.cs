using Compellio.Bcbcti.Models.Documents;

namespace Compellio.Bcbcti.Services.Ingestion;

public abstract record StixReconciliationDecision
{
    private protected StixReconciliationDecision()
    {
    }
    
    public sealed record SkipDecision : StixReconciliationDecision;
    
    public sealed record UpdateDecision : StixReconciliationDecision
    {
        public required string ETag { get; init; }
        public required RegistrationReceipt Receipt { get; init; }
        public required DateTime CompletedAt { get; init; }
        public required string TarId { get; init; }
    }

    public static readonly StixReconciliationDecision Skip = new SkipDecision();
    
    public static StixReconciliationDecision Update(string eTag, RegistrationReceipt receipt, DateTime completedAt, string tarId) =>
        new UpdateDecision { ETag = eTag, Receipt = receipt, CompletedAt = completedAt, TarId = tarId };
}