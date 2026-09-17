using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Compellio.Bcbcti.Services.Ingestion.Registry;
using Compellio.Bcbcti.Services.RegistryApi.Models;

namespace Compellio.Bcbcti.Services.RegistryApi;

// TODO
public class RegistryApiClient : IRegistryApiClient
{
    
    public async Task<TarReceipt> RegisterTarPayload(TarPayload tarPayload)
    {
        // TODO somehow globally configure
        // see BCBCTI.Services.Serialization.Converters.StixJsonConverter
        var stixSerializerOptions = new JsonSerializerOptions()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        stixSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.SnakeCaseLower));

        // TODO WARNING!!! ONLY THE INNER STIX BUNDLE NEEDS TO BE SERIALIZED WITH stixSerializerOptions
        //                 THE REMAINING TAR ENVELOPE SHOULD NOT (snake case, etc.)!
        var data = JsonSerializer.Serialize(tarPayload, stixSerializerOptions);

        Console.WriteLine($"TODO Call Registry API to register: {data}");
        // dummy response (!careful: checksum in hex, not base64)

        return new TarReceipt
        {
            ReceiptId = Guid.NewGuid(),
            Checksum = "0x7A38BF81F383F69433AD6E900D35B3E2385593F76A7B7AB5D4355B8BA41EE24B",
            Version = 0,
            Data = JsonDocument.Parse(data)
        };
    }
    
}