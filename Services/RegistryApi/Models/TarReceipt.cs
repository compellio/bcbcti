using System.Text.Json;

namespace Compellio.Bcbcti.Services.RegistryApi.Models;

public class TarReceipt
{
    public string? Id { get; set; }
    public required Guid ReceiptId { get; set; }
    public required string Checksum { get; set; } // TODO fixme hex checksum => byte[] better type
    public required int Version { get; set; }
    public required JsonDocument Data { get; set; }
    
    // dummy response
    // {"id":"","receipt":"f4804edf-6bbb-41aa-a853-1eb0d55e6c99","data":{"foo":"bar"},"checksum":"0x7A38BF81F383F69433AD6E900D35B3E2385593F76A7B7AB5D4355B8BA41EE24B","version":1,"_sdHashes":[]}
}