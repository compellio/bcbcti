// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Models.Documents;

namespace Compellio.Bcbcti.Services.Ingestion;

public abstract record StixReconciliationDecision
{
    private protected StixReconciliationDecision()
    {
    }

    public sealed record SkipDecision : StixReconciliationDecision;

    public sealed record ReconcileDecision : StixReconciliationDecision
    {
        public required string ETag { get; init; }
        public required RegistrationReceipt Receipt { get; init; }
        public required DateTime CompletedAt { get; init; }
        public required string TarId { get; init; }
    }

    public static readonly StixReconciliationDecision Skip = new SkipDecision();

    public static StixReconciliationDecision Reconcile(string eTag, RegistrationReceipt receipt, DateTime completedAt, string tarId) =>
        new ReconcileDecision { ETag = eTag, Receipt = receipt, CompletedAt = completedAt, TarId = tarId };
}