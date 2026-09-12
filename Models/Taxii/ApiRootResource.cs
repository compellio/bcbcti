namespace Bcbcti.Models.Taxii;

/// <summary>
/// TAXII 4.2.1 API Root Resource
/// </summary>
/// <see href="https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285804"/>
public class ApiRootResource
{
    /// <summary>
    /// A human readable plain text name used to identify this API instance.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// A human readable plain text description for this API Root.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The list of TAXII versions that this API Root is compatible with. The values listed in this property MUST match the media types defined in Section 1.6.8.1 and MUST include the optional version parameter. A value of "application/taxii+json;version=2.1" MUST be included in this list to indicate conformance with this specification.
    /// </summary>
    public required string[] Versions { get; set; }

    /// <summary>
    /// The maximum size of the request body in octets (8-bit bytes) that the server can support. The value of the max_content_length MUST be a positive integer greater than zero. This applies to requests only and is determined by the server. Requests with total body length values smaller than this value MUST NOT result in an HTTP 413 (Request Entity Too Large) response. If for example, the server supported 100 MB of data, the value for this property would be determined by 100*1024*1024 which equals 104,857,600. This property contains useful information for the client when it POSTs requests to the Add Objects endpoint.
    /// </summary>
    public required int MaxContentLength { get; set; }
}