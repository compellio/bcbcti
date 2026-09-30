// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;
using Compellio.Bcbcti.Models.Stix;

namespace Compellio.Bcbcti.Models.Taxii;

public class StixObjectResource : StixObject
{
    [JsonConstructor]
    public StixObjectResource(string type, string id) : base(id)
    {
        Type = type;
    }

    public override string Type { get; }
}