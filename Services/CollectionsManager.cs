using Compellio.Bcbcti.Options;
using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services;

public class CollectionsManager(IOptions<BcbctiOptions> options)
{
    public IReadOnlyList<CollectionOptions> All { get; } = options.Value.Collections;
    
    /// <summary>
    /// Finds a collection by its id or alias
    /// </summary>
    /// <param name="id">A collection id or alias</param>
    public CollectionOptions? Find(string id)
    {
        if (Guid.TryParse(id, out var guid))
        {
            return All.First(collection => collection.Id == guid);
        }
        
        return All.First(collection => collection.Alias == id);
    }
}