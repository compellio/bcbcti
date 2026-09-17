using System.Text.Json.Serialization;

namespace Compellio.Bcbcti.Models.Stix;

/// <summary>
/// 10.7 Hashing Algorithm Vocabulary
/// </summary>
/// <see href="https://docs.oasis-open.org/cti/stix/v2.1/os/stix-v2.1-os.html#_tumklw3o2gyz"/>
public enum StixHashAlgorithm
{
    [JsonStringEnumMemberName("MD5")]
    MD5,
    [JsonStringEnumMemberName("SHA-1")]
    SHA_1,
    [JsonStringEnumMemberName("SHA-256")]
    SHA_256,
    [JsonStringEnumMemberName("SHA-512")]
    SHA_512,
    [JsonStringEnumMemberName("SHA3-256")]
    SHA3_256,
    [JsonStringEnumMemberName("SHA3-512")]
    SHA3_512,
    [JsonStringEnumMemberName("SSDEEP")]
    SSDEEP,
    [JsonStringEnumMemberName("TLSH")]
    TLSH
}