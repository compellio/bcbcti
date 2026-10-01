// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;
using Compellio.Bcbcti.Services.Serialization.Primitives;

namespace Compellio.Bcbcti.Models.Documents;

public enum RegistrationState
{
    Registered, // ok - idle
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
        
        public required DateTime CompletedAt { get; init; }
    }

    public required string ObjectId { get; init; }
    public required Guid CollectionId { get; init; }

    public required string TarId { get; init; }
    public required IReadOnlyList<Version> History { get; init; }

    public required RegistrationState State { get; init; }

    [JsonIgnore]
    public Version? CurrentVersion => History.LastOrDefault(defaultValue: null);

    public static ObjectRegistration Create(Guid collectionId, string objectId, string tarId, Version version)
    {
        return new ObjectRegistration
        {
            ObjectId = objectId, 
            CollectionId = collectionId, 
            State = RegistrationState.Registered, 
            TarId = tarId,
            History = [version]
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
            State = State
        };
    }
}