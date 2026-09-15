using Bcbcti.Services.Storage.Json;

namespace Bcbcti.Repositories;

public abstract class Repository
{
    protected readonly IJsonObjectStore Store;

    protected Repository(IJsonObjectStore store)
    {
        Store = store;
    }
}