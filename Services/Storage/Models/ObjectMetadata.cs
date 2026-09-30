// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Services.Storage.Models;

public class ObjectMetadata : ObjectSummary
{
    public required Uri PublicObjectUrl { get; set; }
    public string? ChecksumSha256 { get; set; }
}