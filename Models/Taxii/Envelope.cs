// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Models.Taxii;

public abstract class Envelope<T>
{
    public required T[] Objects { get; set; }
}