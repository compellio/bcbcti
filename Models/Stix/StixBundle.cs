// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Models.Stix;

public class StixBundle(Guid id) : KnownStixObject("bundle", id)
{
    public required StixObject[] Objects { get; set; }
}