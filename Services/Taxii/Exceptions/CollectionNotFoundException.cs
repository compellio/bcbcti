// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;

namespace Compellio.Bcbcti.Services.Taxii.Exceptions;

public class CollectionNotFoundException(string id) : TaxiiException($"Collection '{id}' not found")
{

    /// <remark>
    /// Replicates ArgumentNullException.ThrowIfNull method
    /// </remark>
    public static void ThrowIfNull([NotNull] object? argument, string id)
    {
        if (argument is null)
        {
            throw new CollectionNotFoundException(id);
        }
    }

};