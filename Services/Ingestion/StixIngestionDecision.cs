// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Models.Documents;

namespace Compellio.Bcbcti.Services.Ingestion;

// union type attempt https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/union
public abstract record StixIngestionDecision
{
    private protected StixIngestionDecision()
    {
    }

    public sealed record CreateDecision : StixIngestionDecision;

    public sealed record AbortDecision : StixIngestionDecision;

    public sealed record UpdateDecision : StixIngestionDecision
    {
        public required string ETag { get; init; }
        public required ObjectRegistration ObjectRegistration { get; init; }
        public required string TarId { get; init; }
    }

    public static readonly StixIngestionDecision Create = new CreateDecision();
    public static readonly StixIngestionDecision Abort = new AbortDecision();

    public static StixIngestionDecision Update(string eTag, ObjectRegistration objectRegistration, string tarId) =>
        new UpdateDecision { ETag = eTag, ObjectRegistration = objectRegistration, TarId = tarId };
}