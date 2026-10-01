// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

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

    public sealed record FailureResolution : StixReconciliationResult
    {
        public string? Message { get; init; }
    };

    public sealed record SkipResolution : StixReconciliationResult;

    public sealed record TimedOutResolution : StixReconciliationResult;

    public static StixReconciliationResult Success(string objectId) => new SuccessResolution { ObjectId = objectId };

    public static StixReconciliationResult Failure(string? message = null) => new FailureResolution { Message = message };

    public static readonly StixReconciliationResult Abort = new AbortResolution();
    public static readonly StixReconciliationResult Skip = new SkipResolution();
    public static readonly StixReconciliationResult TimedOut = new TimedOutResolution();
}