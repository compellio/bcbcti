// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Models.Documents;

public class RegistrationMetadata : VersionMetadata
{
    public required string TarId { get; set; }
}