// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Services.Storage.Models;

public class ListObjectsResponse
{
    public required IAsyncEnumerable<ObjectSummary> Objects { get; set; }
}