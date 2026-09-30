// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Models.Stix;

public class KnownStixObject : StixObject
{
    public KnownStixObject(string type, Guid id) : base($"{type}--{id}")
    {
        Type = type;
    }

    public override string Type { get; }
}