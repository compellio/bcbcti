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
    public JsonElement? Data { get; set; }
}
