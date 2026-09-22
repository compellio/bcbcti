using Compellio.Bcbcti.Models.Documents;
using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Options;
using Compellio.Bcbcti.Repositories;
using Compellio.Bcbcti.Services.Taxii.Mappers;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.Taxii;

public class ManifestService
{
    private readonly TaxiiOptions _taxiiOptions;
    
    private readonly ManifestRepository _manifestRepository;

    public ManifestService(IOptions<TaxiiOptions> taxiiOptions, ManifestRepository manifestRepository)
    {
        _taxiiOptions = taxiiOptions.Value;
        _manifestRepository = manifestRepository;
    }

    public async Task<ManifestResource> GetManifest(DateTime? addedAfter, int? limit, CancellationToken ct = default)
    {
        var take = Math.Min(limit ?? _taxiiOptions.Pagination.DefaultLimit, _taxiiOptions.Pagination.MaxLimit);
        
        var manifest = _manifestRepository.ListManifestEntries(addedAfter);

        var entries = new List<ManifestEntry>(take);
        var more = false;

        await foreach (var manifestObject in manifest.Objects.WithCancellation(ct))
        {
            if (entries.Count == take)
            {
                more = true;
                break;
            }

            var entry = await _manifestRepository.GetManifestEntry(manifestObject.ObjectKey, ct);
            entries.Add(entry.Body);
        }

        var firstAdded = entries.FirstOrDefault()?.CompletedAt;
        var lastAdded = entries.LastOrDefault()?.CompletedAt;

        return ManifestMapper.ToResource(entries, more, firstAdded, lastAdded);
    }
}