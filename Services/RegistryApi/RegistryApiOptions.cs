// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.DataAnnotations;

namespace Compellio.Bcbcti.Services.RegistryApi;

public class RegistryApiOptions
{
    // TODO not required for local instances
    [Required]
    public required string ApiKey { get; set; }
    
    [Required, Url]
    public required string ServiceUrl { get; set; } = "https://registry.api.gateway.compellio.com/";
    
    public string? Network { get; set; }
    public string? IssuerDomain { get; set; }
    public string? WebhookSecret { get; set; }
}
