using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Compellio.Bcbcti.Services.Ingestion.Registry;
using Compellio.Bcbcti.Services.RegistryApi.Models;

namespace Compellio.Bcbcti.Services.RegistryApi;

// TODO
public class RegistryApiClient : IRegistryApiClient
{

    private JsonSerializerOptions _serializerOptions;

    public RegistryApiClient(JsonSerializerOptions serializerOptions/* + http client */)
    {
        _serializerOptions = serializerOptions;
    }
    
    public async Task<TarReceipt> RegisterTarPayload(TarPayload tarPayload)
    {
        var data = JsonSerializer.Serialize(tarPayload, _serializerOptions);

        var sentAt = DateTime.UtcNow;
        Console.WriteLine($"TODO [POST /api/v1/TAR]\n{data}");
        // dummy response (!careful: checksum in hex, not base64)

        return new TarReceipt
        {
            ReceiptId = Guid.NewGuid(),
            Checksum = "0x7A38BF81F383F69433AD6E900D35B3E2385593F76A7B7AB5D4355B8BA41EE24B",
            Version = 0,
            Data = JsonDocument.Parse(data),
            SentAt = sentAt
        };
    }

    public async Task<TarReceipt> UpdateTarPayload(string tarId, TarPayload tarPayload)
    {        
        var data = JsonSerializer.Serialize(tarPayload, _serializerOptions);
        
        var sentAt = DateTime.UtcNow;
        Console.WriteLine($"TODO [PUT /api/v1/TAR/{tarId}]\n{data}");
        
        return new TarReceipt
        {
            ReceiptId = Guid.NewGuid(),
            Checksum = "0x7A38BF81F383F69433AD6E900D35B3E2385593F76A7B7AB5D4355B8BA41EE24B",
            Version = 2,
            Data = JsonDocument.Parse(data),
            SentAt = sentAt
        };
    }

    public Task<TarReceipt> GetTar(Guid receiptId)
    {
        throw new NotImplementedException();
    }

    public Task<TarReceipt> GetTar(string tarId)
    {
        throw new NotImplementedException();
    }
    
}