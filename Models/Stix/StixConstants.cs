// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Models.Stix;

public static class StixConstants
{
    /// <summary>
    /// STIX Cyber-observable Objects UUIDv5 namespace
    /// </summary>
    /// <see href="https://docs.oasis-open.org/cti/stix/v2.1/cs02/stix-v2.1-cs02.html_64yvzeku5a5c"/>
    public static readonly Guid ScoIdentifierUuid5Namespace = new("00abedb4-aa42-466c-9c01-fed23315a9b7");
    
    public const string MediaType = "application/stix+json;version=2.1";
}