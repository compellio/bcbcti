namespace Compellio.Bcbcti.Services.RegistryApi.Models;

public class RegistryResponse
{
    
    /// <summary>
    /// Time the request was sent
    /// </summary>
    public required DateTime SentAt { get; set; }
    
    public required TarReceipt Receipt { get; set; }
    
}