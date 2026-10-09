// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.DataAnnotations;

namespace Compellio.Bcbcti.Services.RegistryApi;

public class RegistryApiOptions
{
    
    [Url]
    public string ServiceUrl { get; set; } = "https://registry.api.gateway.compellio.com/";
    
    public string? ApiKey { get; set; }
    public string? Network { get; set; }
    public string? IssuerDomain { get; set; }
    public string? WebhookSecret { get; set; }

    public bool? Mock { get; set; } = false;
}
