// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Services.Storage.Models;

public class ListObjectsRequest
{
    public required string Prefix { get; set; }
    public string? StartAfter { get; set; }
}