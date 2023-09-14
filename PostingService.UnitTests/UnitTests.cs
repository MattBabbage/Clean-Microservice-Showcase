namespace PostingService.UnitTests;
using StatusAPI;
using MongoDB.Driver;
using EphemeralMongo;
using FluentAssertions;
using Moq;
using StatusDefinition;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.HttpResults;

public class StatusAPITests
{
    //Aim: Test the individual functions/units of the system using mock/fake interfaces
    //This is all implemented using In-Memory equivilants to the "Production" equivilants:
    // AddStackExchangeRedisCache (Container) -> Standard IDistributedCache  
    // MongoDB (Container) -> EphemeralMongo 

    public StatusAPITests() {

        // Status product = new Status("Unit Test Title","Unit Test Content",new StatusLocation(){
        //         City = "Test City",
        //         Region = "Test Region",
        //         Country = ""
        //     }){Id = ""};
        // Status status = new Status{
        //     Id = "",
        //     Title = statusRequest.Title,
        //     Content = statusRequest.Message,
        //     Location = location
        // };
    }

            // var expectedData = new byte[] { 100, 200 };
        // var opts = Options.Create<MemoryDistributedCacheOptions>(new MemoryDistributedCacheOptions());
        // IDistributedCache cache1 = new MemoryDistributedCache(opts);
        // cache1.Set("key1", expectedData);
    [Fact]
    public void GetStatus_WithId()
    {
        // Arrange
        //Setup in memory database
        var runner = MongoRunner.Run();
        var InMemoryDatabase = new MongoClient(runner.ConnectionString).GetDatabase("default");
        InMemoryDatabase.CreateCollection("InMemory_statuses");
        IMongoCollection<Status> collection = InMemoryDatabase.GetCollection<Status>("InMemory_statuses");
        //Create mock data
        Status InMemStatus = new Status(){
            Id = "ecc253b967a1b0067240e139",
            Title="TestStatus",
            Content="TestContent",
            Location = new StatusLocation(){
                City = "TestCity",
                Region = "TestRegion",
                Country = "TestCountry"
            }
        };
        collection.InsertOne(InMemStatus);
        //Act
        var RetrievedStatus = StatusAPI.GetStatus(InMemStatus.Id, collection).Result as Ok<Status>;
        //Assert
        InMemStatus.Should().BeEquivalentTo(RetrievedStatus.Value);
    }
}