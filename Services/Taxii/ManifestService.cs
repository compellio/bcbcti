// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Extensions;
using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services.Taxii.Mappers;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.Taxii;

public class ManifestService
{
    private readonly IOptions<TaxiiOptions> _taxiiOptions;

    private readonly ManifestRepository _manifestRepository;

    public ManifestService(IOptions<TaxiiOptions> taxiiOptions, ManifestRepository manifestRepository)
    {
        _taxiiOptions = taxiiOptions;
        _manifestRepository = manifestRepository;
    }

    public async Task<ManifestResource> GetManifest(DateTime? addedAfter, int? limit, CancellationToken ct = default)
    {
        var take = Math.Min(limit ?? _taxiiOptions.Value.Pagination.DefaultLimit, _taxiiOptions.Value.Pagination.MaxLimit);

        var page = await _manifestRepository.GetManifestPage(take, addedAfter, ct);

        // TODO set ParallelOptions.MaxDegreeOfParallelism based on current store options (e.g. AmazonS3Config.MaxConnectionsPerServer, default = 50)
        var manifestEntries = await page.Items.ParallelSelectAsync(
            async (manifestObject, ctoken) =>
                (await _manifestRepository.GetManifestEntry(manifestObject.ObjectKey, ctoken)).Body, ct);
        
        DateTime? firstAdded = manifestEntries.Count > 0 ? manifestEntries[0].CompletedAt : null;
        DateTime? lastAdded = manifestEntries.Count > 0 ? manifestEntries[^1].CompletedAt : null;

        return ManifestMapper.ToResource(manifestEntries, page.HasMore, firstAdded, lastAdded);
    }
}