// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Models.Stix;
using Compellio.Bcbcti.Models.Taxii;

namespace Compellio.Bcbcti.Services.Taxii.Mappers;

public static class EnvelopeMapper
{
    public static EnvelopeResource ToResource(IReadOnlyList<StixObject> objects, bool more = false,
        DateTime? dateAddedFirst = null, DateTime? dateAddedLast = null) =>
        new()
        {
            More = more, DateAddedFirst = dateAddedFirst, DateAddedLast = dateAddedLast, Objects = objects.ToArray()
        };
}