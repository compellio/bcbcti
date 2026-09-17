using Bcbcti.Services.Ingestion.Registry;
using Bcbcti.Services.RegistryApi.Models;

namespace Bcbcti.Services.RegistryApi;

public interface IRegistryApiClient
{

    public Task<TarReceipt> RegisterTarPayload(TarPayload tarPayload);

}