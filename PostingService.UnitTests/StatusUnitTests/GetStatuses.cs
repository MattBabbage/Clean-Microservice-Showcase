namespace PostingService.UnitTests;
using StatusAPI;
using FluentAssertions;
using StatusDefinition;
using Microsoft.AspNetCore.Http.HttpResults;

public class GetStatusesTests : BaseUnitTest
{
    //GetStatus Tests
    [Fact]
    public async Task GetStatuses_EmptyDB()
    {
        // Arrange
        // Act
        var RetrievedStatuses = await StatusAPI.GetStatuses(Collection, 1, 10) as Ok<List<Status>>;
        // Assert
        // Should get empty array
        RetrievedStatuses.StatusCode.Should().Be(200);
        RetrievedStatuses.Value.Should().BeEquivalentTo(new List<Status>());
    }
    [Fact]
    public async Task GetStatuses_NotEmptyDB()
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
        var RetrievedStatuses = await StatusAPI.GetStatuses(Collection, 1, 10) as Ok<List<Status>>;
        // Assert
        // Should be equivalent to acts as a deep copy check
        RetrievedStatuses.StatusCode.Should().Be(200);
        RetrievedStatuses.Value.Should().BeEquivalentTo(new List<Status>(){InMemStatus});
    }

    //GetStatus Tests
    [Fact]
    public async Task GetStatuses_InvalidPage()
    {
        // Arrange
        // Act
        var RetrievedStatuses = await StatusAPI.GetStatuses(Collection, -1, 10) as BadRequest<string>;
        // Assert
        RetrievedStatuses.StatusCode.Should().Be(400);
    }

        //GetStatus Tests
    [Fact]
    public async void GetStatuses_InvalidPageSize()
    {
        // Arrange
        // Act
        var RetrievedStatuses = await StatusAPI.GetStatuses(Collection, 1, -10) as BadRequest<string>;
        // Assert
        RetrievedStatuses.StatusCode.Should().Be(400);
    }
}
