namespace Compellio.Bcbcti.Options;

public class TaxiiOptions
{
    public string ServerTitle { get; set; } = "BCBCTI TAXII Server";

    public string Title { get; set; } = "Registry API STIX Registration Proxy";

    public string Description { get; set; } =
        "Passthrough TAXII service to forward STIX objects to the Compellio Gateway Registry API";

    /// <summary>
    /// Maximum STIX payload size (see 4.2.1 API Root Resource max_content_length)
    /// </summary>
    /// <see href="https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285804"/>
    /// TODO the server must be configured to reject larger requests (limit request size)
    public long MaxUploadBytes { get; set; } = 10 * 1024 * 1024;
    
    /// <summary>
    /// Maximum STIX objects that can be submitted at once 
    /// </summary>
    /// <remarks>Adjust based on available rate limits for the Registry API and selected storage provider.</remarks>
    public int MaxUploadCount { get; set; } = 100;
    
    public TaxiiPaginationOptions Pagination { get; set; } = new TaxiiPaginationOptions();
    
}