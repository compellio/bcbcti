// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Services.Serialization.Primitives;

namespace Compellio.Bcbcti.Models.Documents;

public class ManifestEntry
{
    public required string ObjectId { get; set; }
    public required string ObjectKey { get; set; }
    public required StixTimestamp ObjectVersion { get; set; }
    
    public required Guid ReceiptId { get; set; }
    public required DateTime CompletedAt { get; set; }
    
    public required RegistrationMetadata RegistrationMetadata { get; set; }
}