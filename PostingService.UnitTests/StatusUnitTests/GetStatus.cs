namespace PostingService.UnitTests;
using StatusAPI;
using FluentAssertions;
using StatusDefinition;
using Microsoft.AspNetCore.Http.HttpResults;

public class GetStatusTests : BaseUnitTest
{
    //GetStatus Tests
    [Fact]
    public async Task GetStatus_WithId()
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
        var RetrievedStatus = await StatusAPI.GetStatus(InMemStatus.Id, Collection) as Ok<Status>;
        // Assert
        // Should be equivalent to acts as a deep copy check
        InMemStatus.Should().BeEquivalentTo(RetrievedStatus.Value);
    }
    
    [Fact]
    public async Task GetStatus_WithWrongId()
    {
        // Arrange
        //No need to create data - testing data not found
        // Act
        var RetrievedStatus = await StatusAPI.GetStatus("ecc253b967a1b0067240e333", Collection) as NotFound<string>;
        // Assert
        // Tested method with data found aswell, works as expected
        RetrievedStatus.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task GetStatus_WithInvalidId()
    {
        // Arrange
        //No need to create data
        // Act
        var RetrievedStatus = await StatusAPI.GetStatus(":)", Collection) as BadRequest<string>;
        // Assert
        // Tested method with data found aswell, works as expected
        RetrievedStatus.StatusCode.Should().Be(400);
    }
}