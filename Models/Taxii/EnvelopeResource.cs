// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Models.Stix;

namespace Compellio.Bcbcti.Models.Taxii;

/// <summary>
/// TAXII 3.7 Envelope Resource
/// </summary>
/// <see href="https://docs.oasis-open.org/cti/taxii/v2.1/os/taxii-v2.1-os.html#_Toc26285797"/>
public class EnvelopeResource : PaginatedEnvelope<StixObject>;
