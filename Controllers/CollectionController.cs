using Bcbcti.Exceptions.Taxii;
using Bcbcti.Models.Stix;
using Bcbcti.Models.Taxii;
using Bcbcti.Services;
using Bcbcti.Services.Storage.Json;
using Microsoft.AspNetCore.Mvc;

namespace Bcbcti.Controllers;

[ApiController]
[Route("/api/collections/{collectionId}")]
[Consumes("application/taxii+json", "application/taxii+json;version=2.1")]
[Produces("application/taxii+json;version=2.1")]
public class CollectionController(CollectionsManager collections, IJsonObjectStore jsonObjectStore) : ControllerBase
{
    [HttpGet(Name = "GetCollection")]
    public CollectionResource Get(string collectionId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);

        return CollectionResource.FromCollectionOptions(collection, true, true);
    }

    [HttpGet(Name = "ListManifests")]
    [Route("/manifest")]
    public ManifestResource ListManifests(string collectionId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);

        // TODO return registry-API status/data for submitted objects + apply filtering
        throw new NotImplementedException();
    }

    [HttpGet(Name = "ListObjects")]
    [Route("/objects")]
    public EnvelopeResource ListObjects(string collectionId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);

        // TODO return submitted objects + apply filtering
        throw new NotImplementedException();
    }

    [HttpPost(Name = "CreateObjects")]
    [Route("/objects")]
    public async Task<StixObjectResource> CreateObjects(string collectionId)
    {
        var collection = collections.Find(collectionId);
        CollectionNotFoundException.ThrowIfNull(collection, collectionId);

        // TODO process input and pass on individual objects to the submission handler

        var statusId = Guid.NewGuid();
        var submittedAt = DateTime.UtcNow;

        var getResp = await jsonObjectStore.GetObjectAsync<Test>("test.json");

        Console.WriteLine(getResp.Body.Foo);
        Console.WriteLine(getResp.Metadata.ETag);
        Console.WriteLine(getResp.Metadata.PublicObjectUrl);
        
        var putResp = await jsonObjectStore.PutObjectAsync<Test>("test2.json", new Test
        {
            Foo = "Foo"
        });
        
        Console.WriteLine(putResp.Metadata.ETag);
        Console.WriteLine(putResp.Metadata.PublicObjectUrl);
        
        
        /*
         * For each stixObject -> call submission handler?
         *   1. canonicalise JSON
         *   2. retrieve: stixObject.id, stixObject.modified (datetime parse)
         *   3. create stixObjectKey ![this needs a storage provider function to insert the domain, etc.]
         */

        throw new NotImplementedException();
    }
}

public class Test
{
    public string? Foo { get; set; }
}