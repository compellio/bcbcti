// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.DataAnnotations;

namespace Compellio.Bcbcti.Options;

public class CollectionOptions
{
    [Required] 
    public required Guid Id { get; set; }

    [Required] 
    public required string Title { get; set; }

    public required string? Alias { get; set; }
}