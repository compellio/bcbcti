namespace Compellio.Bcbcti.Services.Serialization.Json;

public interface IJsonSerializer
{

    public ValueTask<TPayload?> DeserializeAsync<TPayload>(Stream data, CancellationToken ct = default);
    
    public Task SerializeAsync<TPayload>(Stream data, TPayload payload, CancellationToken ct = default);

}