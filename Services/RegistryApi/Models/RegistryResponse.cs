// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

namespace Compellio.Bcbcti.Services.RegistryApi.Models;

public class RegistryResponse
{
    
    /// <summary>
    /// Time the request was sent
    /// </summary>
    public required DateTime SentAt { get; set; }
    
    public required TarReceipt Receipt { get; set; }
    
}