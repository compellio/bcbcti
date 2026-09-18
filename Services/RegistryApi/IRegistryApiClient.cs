using Compellio.Bcbcti.Services.Ingestion.Registry;
using Compellio.Bcbcti.Services.RegistryApi.Models;

namespace Compellio.Bcbcti.Services.RegistryApi;

public interface IRegistryApiClient
{

    public Task<TarReceipt> RegisterTarPayload(TarPayload tarPayload);

    public Task<TarReceipt> UpdateTarPayload(string tarId, TarPayload tarPayload);
    
    /// <summary>
    /// Retrieve a TAR by its id
    /// </summary>
    /// <remarks>Calls GET /api/v1/TAR/{tarID}</remarks>
    /// <returns></returns>
    public Task<TarReceipt> GetTar(string tarId);
    
    /// <summary>
    /// Retrive a TAR by a receipt id
    /// </summary>
    /// <remarks>Calls GET /api/v1/TAR/tarId/{receiptID}</remarks>
    /// <returns></returns>
    public Task<TarReceipt> GetTar(Guid receiptId);

}