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

public class UpdateStatusTests
{
    public IMongoRunner MongoDBExecutor {get; set;}
    public IMongoDatabase InMemoryDatabase {get;set;}
    public IMongoCollection<Status> Collection {get; set;}
    public UpdateStatusTests(){
        MongoDBExecutor = MongoRunner.Run();
        InMemoryDatabase = new MongoClient(MongoDBExecutor.ConnectionString).GetDatabase("default");
        InMemoryDatabase.CreateCollection("InMemory_statuses");
        Collection = InMemoryDatabase.GetCollection<Status>("InMemory_statuses");
    }

    //GetStatus Tests
    [Fact]
    public void UpdateStatus_WithId()
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
        //Insert Data
        Collection.InsertOne(InMemStatus);
        //Edit Data to test update
        InMemStatus.Title = "ChangedTitle";
        // Act
        var RetrievedStatus = StatusAPI.UpdateStatus(InMemStatus.Id, InMemStatus, Collection).Result as Ok<Status>;
        // Assert
        // Should be equivalent to acts as a deep copy check
        InMemStatus.Should().BeEquivalentTo(RetrievedStatus.Value);
    }
    
    [Fact]
    public void UpdateStatus_WithWrongId()
    {
        // Arrange
        string fakeId = "ecc253b967a1b0067240e333";
        // Act
        var RetrievedStatus = StatusAPI.UpdateStatus(fakeId, new Status(), Collection).Result as NotFound<string>;
        // Assert
        // Tested method with data found aswell, works as expected
        RetrievedStatus.StatusCode.Should().Be(404);
    }

    [Fact]
    public void UpdateStatus_WithInvalidId()
    {
        // Arrange
        string invalidId = ":)";
        // Act
        var RetrievedStatus = StatusAPI.UpdateStatus(invalidId, new Status(), Collection).Result as BadRequest<string>;
        // Assert
        // Tested method with data found aswell, works as expected
        RetrievedStatus.StatusCode.Should().Be(400);
    }
}