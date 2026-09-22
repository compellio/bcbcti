using Compellio.Bcbcti.Models.Stix;
using Compellio.Bcbcti.Models.Taxii;
using Compellio.Bcbcti.Options;

namespace Compellio.Bcbcti.Services.Taxii.Mappers;

public static class CollectionMapper
{
    public static CollectionResource ToResource(CollectionOptions options, bool canRead, bool canWrite) =>
        new()
        {
            Id = options.Id,
            Title = options.Title,
            Alias = options.Alias,
            CanRead = canRead,
            CanWrite = canWrite,
            MediaTypes = [StixConstants.MediaType]
        };
}