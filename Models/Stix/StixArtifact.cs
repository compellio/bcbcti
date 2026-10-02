// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;

namespace Compellio.Bcbcti.Models.Stix;

public abstract class StixArtifact(Guid id) : KnownStixObject("artifact", id)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public string? MimeType { get; set; }
}