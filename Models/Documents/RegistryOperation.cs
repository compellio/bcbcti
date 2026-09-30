// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Compellio.Bcbcti.Models.Documents;

public enum RegistryOperationType
{
    Create,
    Update,
    Delete
}

/// <summary>
/// Functions as a pending operation marker 
/// </summary>
public class RegistryOperation
{
    public required Guid CollectionId { get; set; }

    public required Guid JournalId { get; set; }
    public required DateTime SubmittedAt { get; set; } // used to calculate staleness
    
    public required string ObjectId { get; set; }
    
    public required RegistryOperationType OperationType { get; set; }

    public string? ObjectKey { get; set; }

    public Guid? ReceiptId { get; set; }

    [JsonIgnore] 
    [MemberNotNullWhen(true, nameof(ReceiptId))]
    public bool WasSent => ReceiptId.HasValue;

    [JsonIgnore] 
    [MemberNotNullWhen(true, nameof(ObjectKey))]
    public bool HasObject => ObjectKey is not null;
    
    public RegistryOperation WithReceipt(Guid receiptId)
    {
        return new RegistryOperation()
        {
            CollectionId = CollectionId,
            ObjectId = ObjectId,
            JournalId = JournalId,
            SubmittedAt = SubmittedAt,
            OperationType = OperationType,
            
            ObjectKey = ObjectKey,
            ReceiptId = receiptId,
        };
    }
    
    /// <remarks>
    /// The need to update the registry operation with an object key is for the reconciliation to properly be able
    /// to cleanup stored objects if an operation expires (stale)
    /// </remarks>
    public RegistryOperation WithObjectKey(string objectKey)
    {
        return new RegistryOperation()
        {
            CollectionId = CollectionId,
            ObjectId = ObjectId,
            JournalId = JournalId,
            SubmittedAt = SubmittedAt,
            OperationType = OperationType,
            
            ObjectKey = objectKey,
            ReceiptId = ReceiptId,
        };
    }
}