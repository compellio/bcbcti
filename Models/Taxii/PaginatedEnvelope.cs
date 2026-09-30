// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;
using Compellio.Bcbcti.Services.Taxii.Filters;

namespace Compellio.Bcbcti.Models.Taxii;

public abstract class PaginatedEnvelope<T> : Envelope<T>, ITaxiiDateAddedBounds
{
    /// <summary>
    /// This property identifies if there is more content available based on the search criteria. The absence of this property means the value is false.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool More { get; set; } = false;
    
    /// <summary>
    /// This property identifies the server provided value of the next record or set of records in the paginated data set. This property MAY be populated if the more property is set to true.
    /// This value is opaque to the client and represents something that the server knows how to deal with and process.
    /// For example, for a relational database this could be the index autoID, for elastic search it could be the Scroll ID, for other systems it could be a cursor ID, or it could be any string (or int represented as a string) depending on the requirements of the server and what it is doing in the background.
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Next { get; set; }

    [JsonIgnore]
    public DateTime? DateAddedFirst { get; set; }
    
    [JsonIgnore]
    public DateTime? DateAddedLast { get; set;  }
}