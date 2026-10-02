// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.DataAnnotations;

namespace Compellio.Bcbcti.Options;

public class RegistryApiOptions
{
    [Required]
    public required string ApiKey { get; set; }
    
    [Required, Url]
    public string? ServiceUrl { get; set; }
    
    public string? Network { get; set; }
    public string? IssuerDomain { get; set; }
    public string? WebhookSecret { get; set; }
}