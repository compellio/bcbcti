using Bcbcti.Models.Stix;

namespace Bcbcti.Models.Taxii;

/// <summary>
/// TAXII 3.7 Envelope Resource
/// </summary>
/// <see href="https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285797"/>
public class EnvelopeResource
{
    /// <summary>
    /// This property identifies if there is more content available based on the search criteria. The absence of this property means the value is false.
    /// </summary>
    public bool? More { get; set; }

    /// <summary>
    /// This property identifies the server provided value of the next record or set of records in the paginated data set. This property MAY be populated if the more property is set to true.
    /// This value is opaque to the client and represents something that the server knows how to deal with and process.
    /// For example, for a relational database this could be the index autoID, for elastic search it could be the Scroll ID, for other systems it could be a cursor ID, or it could be any string (or int represented as a string) depending on the requirements of the server and what it is doing in the background.
    /// </summary>
    public string? Next { get; set; }

    /// <summary>
    /// This property contains one or more STIX Objects. Objects in this list MUST be a STIX Object (e.g., SDO, SCO, SRO, Language Content object, or a Marking Definition object).
    /// </summary>
    public required StixObject[] Objects { get; set; }

}