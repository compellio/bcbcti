// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.DataAnnotations;

namespace Compellio.Bcbcti.Options;

public class CollectionsOptions
{
    [Required]
    [MinLength(1)]
    public required CollectionOptions[] Collections { get; set; }
}