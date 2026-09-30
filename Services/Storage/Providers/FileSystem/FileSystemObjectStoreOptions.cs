using System.ComponentModel.DataAnnotations;

namespace Compellio.Bcbcti.Services.Storage.Providers.FileSystem;

public class FileSystemObjectStoreOptions
{
    [Required]
    [Url]
    public required string BaseUri { get; set; }

    [Required]
    public required string BasePath { get; set; }
}
