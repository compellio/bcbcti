// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Models.Taxii.Requests;

namespace Compellio.Bcbcti.Services.Taxii.Exceptions;

public class UnsupportedFilteringException() : UnsupportedTaxiiFeatureException($"Filtering is not supported")
{
    /// <remark>
    /// Replicates ArgumentNullException.ThrowIfNull method
    /// </remark>
    public static void ThrowIfMatchPresent(FilteringParameters filters)
    {
        if (filters.Match is not null)
        {
            throw new UnsupportedFilteringException();
        }
    }
}