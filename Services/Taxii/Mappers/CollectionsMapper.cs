using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Options;

namespace Compellio.Bcbcti.Services.Taxii.Mappers;

public static class CollectionsMapper
{
    public static CollectionsResource ToResource(IReadOnlyList<CollectionOptions> options) =>
        new()
        {
            Collections = options
                .Select(collection => CollectionMapper.ToResource(collection, true, true))
                .ToArray()
        };
}