namespace Bcbcti.Models.Taxii;

/// <summary>
/// TAXII 4.1.1 Discovery Resource
/// </summary>
/// <see href="https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285802"/>
public class DiscoveryResource
{
    /// <summary>
    /// A human readable plain text name used to identify this server.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// A human readable plain text description for this server.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// The human readable plain text contact information for this server and/or the administrator of this server.
    /// </summary>
    public string? Contact { get; set; }

    /// <summary>
    /// The default API Root that a TAXII Client MAY use. Absence of this property indicates that there is no default API Root. The default API Root MUST be an item in api_roots.
    /// </summary>
    public string? Default { get; set; }
    
    /// <summary>
    /// A list of URLs that identify known API Roots. This list MAY be filtered on a per-client basis.
    /// API Root URLs MUST be HTTPS absolute URLs or relative URLs. API Root relative URLs MUST begin with a single `/` character and MUST NOT begin with `//` or '../". API Root URLs MUST NOT contain a URL query component.
    /// </summary>
    public required string[] ApiRoots { get; set; }
}