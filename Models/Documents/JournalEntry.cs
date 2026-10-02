// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
using Compellio.Bcbcti.Services.Serialization.Primitives;

namespace Compellio.Bcbcti.Models.Documents;

public class JournalEntry
{
    public class Object
    {
        public required string ObjectId { get; set; } // STIX object id
        public required StixTimestamp ObjectVersion { get; set; } // STIX object version per TAXII's specification

        public Guid? ReceiptId { get; set; }
        public string? ObjectKey { get; set; }

        // if submission failed (before a receipt could be created - set <=> ReceiptId and ObjectKey null)
        public string? SubmitFailureReason { get; set; }

        [JsonIgnore]
        [MemberNotNullWhen(true, nameof(SubmitFailureReason))] 
        [MemberNotNullWhen(false, nameof(ReceiptId), nameof(ObjectId))]
        public bool HasFailedEarly => SubmitFailureReason is not null;
    }

    public required Guid Id { get; set; }
    public required Guid CollectionId { get; set; }
    public required DateTime RequestTimestamp { get; set; }
    public required Object[] Objects { get; set; }
}