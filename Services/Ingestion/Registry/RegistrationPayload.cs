// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Models.Stix;

namespace Compellio.Bcbcti.Services.Ingestion.Registry;

public class RegistrationPayload
{
    public required StixArtifact Artifact { get; set; }
    public required StixBundle Bundle { get; set; }
    public required TarPayload Payload { get; set; }
};