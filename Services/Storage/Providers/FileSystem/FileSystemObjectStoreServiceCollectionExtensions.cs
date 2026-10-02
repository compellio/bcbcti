// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Microsoft.Extensions.Options;

namespace Compellio.Bcbcti.Services.Storage.Providers.FileSystem;

public static class FileSystemObjectStoreServiceCollectionExtensions
{
    public static IServiceCollection AddFileSystemObjectStore(this IServiceCollection services,
        Action<FileSystemObjectStoreOptions> configure)
    {
        services.AddOptions<FileSystemObjectStoreOptions>().Configure(configure).ValidateDataAnnotations().ValidateOnStart();

        services.AddSingleton<FileSystemObjectStore>(serviceProvider =>
        {
            var options = serviceProvider.GetRequiredService<IOptions<FileSystemObjectStoreOptions>>().Value;
            return new FileSystemObjectStore(options);
        });

        return services;
    }
}