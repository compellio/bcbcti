// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Services.Storage.Models;

public class PutObjectRequest
{
    public required string ObjectKey { get; set; }
    public required Stream InputStream { get; set; }
    public byte[]? ChecksumSHA256 { get; set; }
    public Condition Condition { get; set; } = Condition.None;
}