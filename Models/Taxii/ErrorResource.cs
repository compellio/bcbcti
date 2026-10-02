// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Models.Taxii;

// TODO configure server to return TAXII error resources for TAXII routes 
public class ErrorResource
{
    /// <summary>
    /// A human readable plain text title for this error.
    /// </summary>
    public required string Title { get; set; }
    
    /// <summary>
    /// A human readable plain text description that gives details about the error or problem that was encountered by the application.
    /// </summary>
    public string? Description { get; set; }
    
    /// <summary>
    /// An identifier for this particular error instance. A TAXII Server might choose to assign each error occurrence its own identifier in order to facilitate debugging.
    /// </summary>
    public string? ErrorId  { get; set; }
    
    /// <summary>
    /// The error code for this error type. A TAXII Server might choose to assign a common error code to all errors of the same type. Error codes are application-specific and not intended to be meaningful across different TAXII Servers.
    /// </summary>
    public string? ErrorCode { get; set; }
    
    /// <summary>
    /// The HTTP status code applicable to this error. If this property is provided it MUST match the HTTP status code found in the HTTP header.
    /// </summary>
    public string? HttpStatus { get; set; }
    
    /// <summary>
    /// A URL that points to additional details. For example, this could be a URL pointing to a knowledge base article describing the error code. Absence of this property indicates that there are no additional details.
    /// </summary>
    public string? ExternalDetails { get; set; }

    /// <summary>
    /// The details property captures additional server-specific details about the error. The keys and values are determined by the TAXII Server and MAY be any valid JSON object structure.
    /// </summary>
    public Dictionary<string, string>? Details { get; set; }
}