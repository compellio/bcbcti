// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Services.Storage.Models;

public class DeleteObjectRequest
{
    public required string ObjectKey { get; set; }
    public Condition Condition { get; set; } = Condition.None;
}