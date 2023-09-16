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

public class GetStatusesTests
{
    public IMongoRunner MongoDBExecutor {get; set;}
    public IMongoDatabase InMemoryDatabase {get;set;}
    public IMongoCollection<Status> Collection {get; set;}
    public GetStatusesTests(){
        MongoDBExecutor = MongoRunner.Run();
        InMemoryDatabase = new MongoClient(MongoDBExecutor.ConnectionString).GetDatabase("default");
        InMemoryDatabase.CreateCollection("InMemory_statuses");
        Collection = InMemoryDatabase.GetCollection<Status>("InMemory_statuses");
    }

    //GetStatus Tests
    [Fact]
    public void GetStatuses_EmptyDB()
    {
        // Arrange
        // Act
        var RetrievedStatuses = StatusAPI.GetStatuses(Collection, 1, 10).Result as Ok<List<Status>>;
        // Assert
        // Should get empty array
        RetrievedStatuses.StatusCode.Should().Be(200);
        RetrievedStatuses.Value.Should().BeEquivalentTo(new List<Status>());
    }
    [Fact]
    public void GetStatuses_NotEmptyDB()
    {
        // Arrange
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
        Collection.InsertOne(InMemStatus);
        // Act
        var RetrievedStatuses = StatusAPI.GetStatuses(Collection, 1, 10).Result as Ok<List<Status>>;
        // Assert
        // Should be equivalent to acts as a deep copy check
        RetrievedStatuses.StatusCode.Should().Be(200);
        RetrievedStatuses.Value.Should().BeEquivalentTo(new List<Status>(){InMemStatus});
    }

    //GetStatus Tests
    [Fact]
    public void GetStatuses_InvalidPage()
    {
        // Arrange
        // Act
        var RetrievedStatuses = StatusAPI.GetStatuses(Collection, -1, 10).Result as BadRequest<string>;
        // Assert
        RetrievedStatuses.StatusCode.Should().Be(400);
    }

        //GetStatus Tests
    [Fact]
    public void GetStatuses_InvalidPageSize()
    {
        // Arrange
        // Act
        var RetrievedStatuses = StatusAPI.GetStatuses(Collection, 1, -10).Result as BadRequest<string>;
        // Assert
        RetrievedStatuses.StatusCode.Should().Be(400);
    }
}
