// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Options;

public class IngestionOptions
{
    public TimeSpan ReconciliationFrequency { get; set; } = TimeSpan.FromMinutes(30);
    public TimeSpan PendingOperationTimeout { get; set; } = TimeSpan.FromHours(4);
    public int? MaxConcurrentIngestions { get; set; } = null;
}
