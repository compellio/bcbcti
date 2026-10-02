// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Models.Stix;

namespace Compellio.Bcbcti.Services.Ingestion;

public enum IngestionResultResolution
{
    Success,
    Failure,
    Abort
}

// TODO refactor into "union" like StixReconciliationResult
public class StixIngestionResult
{
    public required IngestionResultResolution Resolution { get; set; }
    public required StixObject StixObject { get; set; }

    public RegistrationReceipt? RegistrationReceipt { get; set; }
    
    public string? ResolutionFailureMessage { get; set; }
}
