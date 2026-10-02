// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Compellio.Bcbcti.Services.Serialization.Primitives;

namespace Compellio.Bcbcti.Models.Documents;

// TODO-REVIEW placement & scope
public enum RegistrationReceiptState
{
    Sent,
    Succeeded,
    Failed
}

public class RegistrationReceipt
{
    // System props
    public required Guid ReceiptId { get; set; }
    public required Guid JournalId { get; set; }
    public required Guid CollectionId { get; set; }

    // Registration props
    public required RegistryOperationType OperationType { get; set; }
    public required RegistrationReceiptState State { get; set; }

    // TAXII-relevant props
    /// <summary>
    /// The STIX Object identifier
    /// </summary>
    public required string ObjectId { get; set; }

    public required string ObjectKey { get; set; }

    /// <summary>
    /// The STIX Object version per TAXII's specification.
    /// </summary>
    /// <remarks>For STIX objects the version MUST be the STIX modified timestamp Property. If a STIX object is not versioned (and therefore does not have a modified timestamp), the server MUST use the created timestamp. If the STIX object does not have a created or modified timestamp then the server SHOULD use a value for the version that is consistent to the server.</remarks>
    public required StixTimestamp ObjectVersion { get; set; }

    // History props
    public required DateTime SubmittedAt { get; set; } // when BCBCTI server received the object
    public required DateTime SentAt { get; set; } // when Registry API was called
    public DateTime? CompletedAt { get; set; } // when Registry API came back (webhook)

    public string? FailureReason { get; set; }

    public required VersionMetadata Metadata { get; set; }

    [JsonIgnore]
    [MemberNotNullWhen(true, nameof(CompletedAt))]
    public bool IsCompleted => CompletedAt.HasValue;

    [JsonIgnore]
    [MemberNotNullWhen(true, nameof(FailureReason))]
    public bool HasFailed => FailureReason is not null;

    public RegistrationReceipt AsCompleted(DateTime completedAt, string tarId)
    {
        return new RegistrationReceipt()
        {
            ReceiptId = ReceiptId,
            JournalId = JournalId,
            CollectionId = CollectionId,

            OperationType = OperationType,
            State = RegistrationReceiptState.Succeeded,

            ObjectId = ObjectId,
            ObjectKey = ObjectKey,
            ObjectVersion = ObjectVersion,

            SubmittedAt = SubmittedAt,
            SentAt = SentAt,
            CompletedAt = completedAt,

            Metadata = new RegistrationMetadata()
            {
                TarId = tarId,
                RegistryChecksum = Metadata.RegistryChecksum,
                Version = Metadata.Version,
            },
        };
    }

    public RegistrationReceipt AsFailed(DateTime completedAt, string failureReason)
    {
        return new RegistrationReceipt()
        {
            ReceiptId = ReceiptId,
            JournalId = JournalId,
            CollectionId = CollectionId,

            OperationType = OperationType,
            State = RegistrationReceiptState.Failed,

            ObjectId = ObjectId,
            ObjectKey = ObjectKey,
            ObjectVersion = ObjectVersion,

            SubmittedAt = SubmittedAt,
            SentAt = SentAt,
            CompletedAt = completedAt,
            
            FailureReason = failureReason,

            Metadata = Metadata,
        };
    }
}