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