// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Compellio.Bcbcti.Services.RegistryApi.Models;

public class TarReceipt
{
    [JsonPropertyName("id")]
    public string? Id { get; set; }
    
    [JsonPropertyName("receipt")]
    public required Guid ReceiptId { get; set; }
    
    [JsonPropertyName("checksum")]
    public required string Checksum { get; set; }
    
    [JsonPropertyName("version")]
    public required int Version { get; set; }
    
    [JsonPropertyName("data")]
    public required JsonElement Data { get; set; }
    
    // dummy response
    // {"id":"","receipt":"f4804edf-6bbb-41aa-a853-1eb0d55e6c99","data":{"foo":"bar"},"checksum":"0x7A38BF81F383F69433AD6E900D35B3E2385593F76A7B7AB5D4355B8BA41EE24B","version":1,"_sdHashes":[]}
}
