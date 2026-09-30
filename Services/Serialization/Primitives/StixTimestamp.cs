// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.Text.Json.Serialization;
using Compellio.Bcbcti.Extensions;
using Compellio.Bcbcti.Services.Serialization.Json.Converters;

namespace Compellio.Bcbcti.Services.Serialization.Primitives;

/// <remarks>DateTime wrapper that preservers raw value for round-trip consistency</remarks>
/// <see href="https://docs.oasis-open.org/cti/stix/v2.1/cs02/stix-v2.1-cs02.html#_ksbm2nost85y"/>
[JsonConverter(typeof(StixTimestampConverter))]
public readonly record struct StixTimestamp(string Raw)
{
    public DateTime Parsed => DateTime.Parse(Raw);

    /// <summary>
    /// Builds a StixTimestamp from a DateTime instance.
    /// </summary>
    /// <remarks>
    /// Using TAXII's microsecond precision which is compatible with STIX's timestamp representation.
    /// </remarks>
    /// <see href="https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285787"/>
    public static StixTimestamp FromDateTime(DateTime dateTime)
    {
        return new StixTimestamp(dateTime.ToTaxii());
    }
}