namespace StatusAPI;
using StatusDefinition;
using MongoDB.Driver;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Swashbuckle.AspNetCore.Annotations;
using MongoDB.Bson;
using System.Net;

public static class StatusAPI
{
    //Separation into seperate class to make minimal api more easily readable
    public static void ConfigureAPI(this WebApplication app)
    {
        //Declare all endpoints
        app.MapPost("/Status", (StatusRequest status, IHttpClientFactory httpClientFactory, IMongoCollection<Status> collection, IDistributedCache cache, IConfiguration config) => 
                                CreateStatus(status, httpClientFactory, collection, cache, config))
                                .WithMetadata(new SwaggerOperationAttribute("Post a Status", "This allows users to post a status"))
                                .RequireRateLimiting("fixed");

        app.MapGet("/Statuses", (IMongoCollection<Status> collection, int page, int pageSize) => GetStatuses(collection, page, pageSize))
                                .WithMetadata(new SwaggerOperationAttribute("Get all statuses in a paged fashion", "Get all data using a page and page size"))
                                .RequireRateLimiting("fixed")
                                .CacheOutput("OutputCacheDemonstrator"); //Only caching output here as this is the only return with possible large data packet

        app.MapGet("/Status/{id}", (string id, IMongoCollection<Status> collection) =>  GetStatus(id, collection))
                                .WithMetadata(new SwaggerOperationAttribute("Get singular status", "Get a singular status based on id"))
                                .RequireRateLimiting("fixed");

        app.MapPut("/Status/{id}", (string id, Status updatedStatus,IMongoCollection<Status> collection) =>  UpdateStatus(id, updatedStatus, collection))
                                .WithMetadata(new SwaggerOperationAttribute("Update a status", "Update status based on ID"))
                                .RequireRateLimiting("fixed");

        app.MapDelete("/Status/{id}", (string id, IMongoCollection<Status> collection) =>  DeleteStatus(id, collection))
                                .WithMetadata(new SwaggerOperationAttribute("Delete a status", "Delete one status based on id"))
                                .RequireRateLimiting("fixed");
    }

    public static async Task<IResult> CreateStatus(StatusRequest statusRequest, IHttpClientFactory locationClientFactory, IMongoCollection<Status> collection, IDistributedCache cache,  IConfiguration config){
        //Validate IP address
        if (!IPAddress.TryParse(statusRequest.IPAddress, out _))
            return Results.BadRequest("Bad ip address");
        //Get the location from the IP Address
        StatusLocation? location = await GetLocation(statusRequest, cache, locationClientFactory, config);
        //Check location found
        if (location is null)
            return Results.NotFound("Location not found");
        //Create, Insert and Return new status
        Status status = new Status{
            Title = statusRequest.Title,
            Content = statusRequest.Message,
            Location = location
        };
        await collection.InsertOneAsync(status);
        return Results.Ok(status);
    }

    public static async Task<StatusLocation?> GetLocation(StatusRequest statusRequest, IDistributedCache cache, IHttpClientFactory locationClientFactory, IConfiguration config){
            //Declare Location
            StatusLocation? location;
            //Check Cache for IP Lookup
            var cachedBinaryLocation = await cache.GetAsync(statusRequest.IPAddress);
            if (cachedBinaryLocation != null)
            {
                location = JsonSerializer.Deserialize<StatusLocation>(Encoding.UTF8.GetString(cachedBinaryLocation));
            }
            else
            {
                //Create and use named http client
                var client = locationClientFactory.CreateClient("Location");
                HttpResponseMessage locationResponse = await client.GetAsync($"?api_key={config["Location:ApiKey"]}&ip_address={statusRequest.IPAddress}");
                //Create Object
                location = JsonSerializer.Deserialize<StatusLocation>(await locationResponse.Content.ReadAsStringAsync());
                var options = new DistributedCacheEntryOptions(); // create options object
                //Save 30 Min cache on IP, people may want to write many posts in a row, saves api calls
                options.SetSlidingExpiration(TimeSpan.FromMinutes(30)); 
                await cache.SetAsync(statusRequest.IPAddress, Encoding.UTF8.GetBytes(JsonSerializer.Serialize(location)), options);
            }
            return location;
    }

    public static async Task<IResult> GetStatuses(IMongoCollection<Status> collection, int page, int pageSize){
        //Validate search query can happen
        if (page < 1 || pageSize < 1)
             return Results.BadRequest($"Invalid page");
        //Select all data withing the page limitations
        //FindAsync -> this returns cursor which allows for better queries on big data (Future Improvement)
        var data = await collection.Find(_ => true)
                            .Skip((page - 1) * pageSize)
                            .Limit(pageSize)
                            .ToListAsync();
        //Check return exists and contains data
        if (data is null || !data.Any())
            return Results.Ok(new List<Status>());
        return Results.Ok(data);
    }

    public static async Task<IResult> GetStatus(string id, IMongoCollection<Status> collection){
        //Validate id - No possibility of injection but helps user in diagnosis
        if (!ObjectId.TryParse(id, out _))
            return Results.BadRequest("Invalid Id");
        //Select data where Id is found    - no FindOne
        var data = collection.Find(s => s.Id == id).SingleOrDefault<Status>();
        //Check return exists and contains data
        if (data is null)
            return Results.NotFound("Not found");
        return Results.Ok(data);
    }

    public static async Task<IResult> UpdateStatus(string id, Status updatedStatus, IMongoCollection<Status> collection){
        //Validate id - No possibility of injection but helps user in diagnosis
        if (!ObjectId.TryParse(id, out _))
            return Results.BadRequest("Invalid Id");
        //Create Filter and Update Spec
        var SearchOneFilter = Builders<Status>.Filter.Eq(x => x.Id, id);
        var updateDefinition = Builders<Status>.Update
                .Set(s => s.Title, updatedStatus.Title)
                .Set(s => s.Content, updatedStatus.Content)
                .Set(s => s.Location, updatedStatus.Location);
        
        var replaceResult = await collection.UpdateOneAsync(SearchOneFilter, updateDefinition);

        if (replaceResult.ModifiedCount == 1) //Acknowledgement doesnt work on deletion
            return Results.Ok(updatedStatus);
        return Results.NotFound("Nothing Found / No Update Made");
    }
    public static async Task<IResult> DeleteStatus(string id, IMongoCollection<Status> collection){
        //Validate id - No possibility of injection but helps user in diagnosis
        if (!ObjectId.TryParse(id, out _))
            return Results.BadRequest("Invalid Id");
        //Delete data with id
        var deletion = collection.DeleteOne(x => x.Id == id);
        //Deleted successfully or not found are only 2 possible outcomes with this db structure
        if (deletion.DeletedCount == 1) 
            return Results.Ok("Deleted");
        //Not Deleted, not found
        return Results.NotFound("Not Found");
    }
}