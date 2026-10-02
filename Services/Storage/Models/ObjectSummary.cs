// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Services.Storage.Models;

public class ObjectSummary
{
    public required string ObjectKey { get; set; }
    public required string ETag { get; set; }
    public DateTime? LastModified { get; set; }
}