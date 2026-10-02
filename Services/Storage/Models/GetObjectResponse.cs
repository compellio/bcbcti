// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Services.Storage.Models;

public class GetObjectResponse<TBody>
{
    public required TBody Body { get; set; }
    public required ObjectMetadata Metadata { get; set; }
}