using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Compellio.Bcbcti.Models.Stix;
using Compellio.Bcbcti.Services.Serialization.Primitives;

namespace Compellio.Bcbcti.Models.Documents;

public enum RegistrationState
{
    Creating, // awaiting callback for 1st registration
    Updating, // awaiting callback for update registration
    Registered, // ok - idle
    Deleting,
    Deleted
}

public class ObjectRegistration
{
    public class Version
    {
        public required Guid ReceiptId { get; init; }
        public required int TarVersion { get; init; }

        public required string ObjectKey { get; init; }
        public required StixTimestamp ObjectVersion { get; init; }

        public required string ManifestKey { get; init; }
        
        public required DateTime CompletedAt { get; init; } // = to calculate TAXII DateAdded
    }

    public required string ObjectId { get; init; }
    public required Guid CollectionId { get; init; }

    public string? TarId { get; init; }
    public required IReadOnlyList<Version> History { get; init; }

    public required RegistrationState State { get; init; }

    [JsonIgnore]
    public Version? CurrentVersion => History.LastOrDefault(defaultValue: null);

    [JsonIgnore]
    [MemberNotNullWhen(true, nameof(TarId), nameof(CurrentVersion))]
    public bool IsRegistered => State is RegistrationState.Registered;

    [JsonIgnore]
    public bool IsPending =>
        State is RegistrationState.Creating or RegistrationState.Updating or RegistrationState.Deleting;

    public static ObjectRegistration Create(StixObject stixObject, Guid collectionId)
    {
        return new ObjectRegistration
        {
            ObjectId = stixObject.Id, CollectionId = collectionId, State = RegistrationState.Creating, History = []
        };
    }

    public ObjectRegistration AsCreated(string tarId, Version version)
    {
        return new ObjectRegistration()
        {
            ObjectId = ObjectId,
            CollectionId = CollectionId,
            State = RegistrationState.Registered,
            TarId = tarId,
            History = [version]
        };
    }

    public ObjectRegistration AsUpdating()
    {
        return new ObjectRegistration
        {
            ObjectId = ObjectId,
            CollectionId = CollectionId,
            TarId = TarId,
            History = History,
            State = RegistrationState.Updating
        };
    }

    public ObjectRegistration AsUpdated(Version version)
    {
        return new ObjectRegistration
        {
            ObjectId = ObjectId,
            CollectionId = CollectionId,
            TarId = TarId,
            History = [.. History, version],
            State = RegistrationState.Registered
        };
    }
}