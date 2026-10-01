// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;
using Compellio.Bcbcti.Services.Serialization.Primitives;

namespace Compellio.Bcbcti.Models.Taxii;

/// <summary>
/// TAXII 4.3.1 Status Details Resource
/// </summary>
/// <see href="https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285806"/>
public class StatusDetailsResource
{
    /// <summary>
    /// The identifier of the object that succeed, is pending, or failed to be created. For STIX objects the id MUST be the STIX Object id. For object types that do not have their own identifier, the server MAY use any value as the id.
    /// </summary>
    public required string Id { get; set; }

    /// <summary>
    /// The version of the object that succeeded, is pending, or failed to be created. For STIX objects the version MUST be the STIX modified timestamp Property. If a STIX object is not versioned (and therefore does not have a modified timestamp), the server MUST use the created timestamp. If the STIX object does not have a created or modified timestamp then the server SHOULD use a value for the version that is consistent to the server.
    /// </summary>
    public required StixTimestamp Version { get; set; }

    /// <summary>
    /// A message indicating more information about the object being created, its pending state, or why the object failed to be created.
    /// </summary>
    public string? Message { get; set; }
    
    [JsonPropertyName("x_bcbcti_registry_receipt_id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Guid? ReceiptId { get; set; }
}