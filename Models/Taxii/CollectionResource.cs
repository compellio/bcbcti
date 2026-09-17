using Compellio.Bcbcti.Options;

namespace Compellio.Bcbcti.Models.Taxii;

/// <summary>
/// TAXII 5.2.1 Collection Resource
/// </summary>
/// <see href="https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285811"/>
public class CollectionResource
{
    /// <summary>
    /// The id property universally and uniquely identifies this Collection. It is used in the Get Collection Endpoint (see section 5.2) as the {id} parameter to retrieve the Collection.
    /// </summary>
    public required Guid Id { get; set; }

    /// <summary>
    /// A human readable plain text title used to identify this Collection.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// A human readable plain text description for this Collection.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// A human readable collection name that can be used on systems to alias a collection ID. This could be used by organizations that want to preconfigure a known collection of data, regardless of the underlying collection ID that is configured on a specific implementations.
    /// If defined, the alias MUST be unique within a single api-root on a single TAXII server. There is no guarantee that an alias is globally unique across api-roots or TAXII server instances.
    /// </summary>
    public string? Alias { get; set; }

    /// <summary>
    /// Indicates if the requester can read (i.e., GET) objects from this Collection. If true, users are allowed to access the Get Objects, Get an Object, or Get Object Manifests endpoints for this Collection. If false, users are not allowed to access these endpoints.
    /// </summary>
    public required bool CanRead { get; set; }

    /// <summary>
    /// Indicates if the requester can write (i.e., POST) objects to this Collection. If true, users are allowed to access the Add Objects endpoint for this Collection. If false, users are not allowed to access this endpoint.
    /// </summary>
    public required bool CanWrite { get; set; }

    /// <summary>
    /// A list of supported media types for Objects in this Collection. Absence of this property is equivalent to a single-value list containing  "application/stix+json". This list MUST describe all media types that the Collection can store.
    /// </summary>
    public string[]? MediaTypes { get; set; }

    // TODO review location -> new options->resource mapping utility class?
    // TODO FIXME canRead/canWrite should be defined by authentication state (when/if implemented)
    public static CollectionResource FromCollectionOptions(CollectionOptions options, bool canRead, bool canWrite) =>
        new()
        {
            Id = options.Id,
            Title = options.Title,
            Alias = options.Alias,
            CanRead = canRead,
            CanWrite = canWrite,
            MediaTypes = ["application/stix+json;version=2.1"]
        };
}