namespace Bcbcti.Options;

public class CollectionOptions
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string? Alias { get; set; }
    public required RegistryApiOptions RegistryApi { get; set; }
    public required StorageOptions Storage { get; set; }
}
