// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Models.Taxii;

namespace Compellio.Bcbcti.Services.Taxii.Mappers;

public static class ManifestMapper
{
    public static ManifestResource
        ToResource(IReadOnlyList<ManifestEntry> manifestEntries, bool more = false, DateTime? dateAddedFirst = null,
            DateTime? dateAddedLast = null) =>
        new()
        {
            More = more,
            DateAddedFirst = dateAddedFirst,
            DateAddedLast = dateAddedLast,
            Objects = manifestEntries.Select(entry => new ManifestResource.Record
                {
                    Id = entry.ObjectId,
                    DateAdded = entry.CompletedAt,
                    Version = entry.ObjectVersion,
                    TarId = entry.RegistrationMetadata.TarId,
                    ReceiptId = entry.ReceiptId,
                    RegistryVersion = entry.RegistrationMetadata.Version
                })
                .ToArray()
        };
}