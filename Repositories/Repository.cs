// SPDX-FileCopyrightText: Copyright (c) 2026 Compellio S.A.
// SPDX-License-Identifier: Apache-2.0

using Compellio.Bcbcti.Services.Storage.Json;

namespace Compellio.Bcbcti.Repositories;

public abstract class Repository
{
    protected readonly IJsonObjectStore Store;

    protected Repository(IJsonObjectStore store)
    {
        Store = store;
    }
}