// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Microsoft.AspNetCore.Mvc;

namespace Compellio.Bcbcti.Models.Taxii.Requests;

public class FilteringParameters
{
    [FromQuery(Name = "added_after")]
    public DateTime? AddedAfter { get; set; }
    
    public int? Limit { get; set; }
    
    public string? Next { get; set; }
    
    // TODO how to type mapping? { match[id]= -> string but match[version]= -> date }
    [FromQuery(Name = "match")]
    public IDictionary<string, string>? Match { get; set; }
}