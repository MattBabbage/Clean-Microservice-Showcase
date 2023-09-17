namespace PostingService.UnitTests;
using StatusAPI;
using FluentAssertions;
using StatusDefinition;
using Microsoft.AspNetCore.Http.HttpResults;

public class UpdateStatusTests : BaseUnitTest
{
    //GetStatus Tests
    [Fact]
    public async Task UpdateStatus_WithId()
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
        var RetrievedStatus = await StatusAPI.UpdateStatus(InMemStatus.Id, InMemStatus, Collection) as Ok<Status>;
        // Assert
        // Should be equivalent to acts as a deep copy check
        InMemStatus.Should().BeEquivalentTo(RetrievedStatus.Value);
    }
    
    [Fact]
    public async Task UpdateStatus_WithWrongId()
    {
        // Arrange
        string fakeId = "ecc253b967a1b0067240e333";
        // Act
        var RetrievedStatus = await StatusAPI.UpdateStatus(fakeId, new Status(), Collection) as NotFound<string>;
        // Assert
        // Tested method with data found aswell, works as expected
        RetrievedStatus.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task UpdateStatus_WithInvalidId()
    {
        // Arrange
        string invalidId = ":)";
        // Act
        var RetrievedStatus = await StatusAPI.UpdateStatus(invalidId, new Status(), Collection) as BadRequest<string>;
        // Assert
        // Tested method with data found aswell, works as expected
        RetrievedStatus.StatusCode.Should().Be(400);
    }
}