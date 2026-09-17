using Compellio.Bcbcti.Services.Ingestion.Registry;
using Compellio.Bcbcti.Services.RegistryApi.Models;

namespace Compellio.Bcbcti.Services.RegistryApi;

public interface IRegistryApiClient
{

    public Task<TarReceipt> RegisterTarPayload(TarPayload tarPayload);

}