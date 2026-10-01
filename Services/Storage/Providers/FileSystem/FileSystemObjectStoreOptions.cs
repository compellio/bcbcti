// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using System.ComponentModel.DataAnnotations;

namespace Compellio.Bcbcti.Services.Storage.Providers.FileSystem;

public class FileSystemObjectStoreOptions
{
    [Required]
    [Url]
    public required string PublicBaseUrl { get; set; }

    [Required]
    public required string BasePath { get; set; }
}
