using System.Text.Json.Serialization;
using Compellio.Bcbcti.Services.Serialization.Primitives;

namespace Compellio.Bcbcti.Models.Taxii;

public class ManifestResource : PaginatedEnvelope<ManifestResource.Record>
{
    public class Record
    {
        public required string Id { get; set; }
        public required DateTime DateAdded { get; set; }
        public required StixTimestamp Version { get; set; }

        [JsonPropertyName("x_bcbcti_registry_tar_id")]
        public required string TarId { get; set; }

        [JsonPropertyName("x_bcbcti_registry_receipt_id")]
        public required Guid ReceiptId { get; set; }

        [JsonPropertyName("x_bcbcti_registry_version_id")]
        public required int RegistryVersion { get; set; }
    }
}