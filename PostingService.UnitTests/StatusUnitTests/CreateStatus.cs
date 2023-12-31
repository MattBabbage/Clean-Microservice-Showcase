namespace PostingService.UnitTests;
using StatusAPI;
using FluentAssertions;
using StatusDefinition;
using Microsoft.AspNetCore.Http.HttpResults;

public class CreateStatusTests : BaseUnitTest
{
    [Fact]
    public async Task CreateStatus_Standard()
    {
        // Arrange
        //No need to create data - testing data not found
        StatusRequest statusRequest = new StatusRequest(){
            IPAddress = "1.0.0.127",
            Title = "",
            Message = ""
        };
        // Act
        var creationResponse = await StatusAPI.CreateStatus(statusRequest, MockHttpClientFactory.Object, Collection, MockRedisCache, MockConfiguration) as Ok<Status>;
        // Assert
        // Tested method with data found aswell, works as expected
        creationResponse.StatusCode.Should().Be(200);
        creationResponse.Value.Title.Should().Be(statusRequest.Title);
    }

    [Fact]
    public async Task CreateStatus_BadIp()
    {
        StatusRequest statusRequest = new StatusRequest(){
            IPAddress = "NotAnIp",
            Title = "",
            Message = ""
        };
        // Act
        var creationResponse = await StatusAPI.CreateStatus(statusRequest, MockHttpClientFactory.Object, Collection, MockRedisCache, MockConfiguration) as BadRequest<string>;
        // Assert
        // Tested method with data found aswell, works as expected
        creationResponse.StatusCode.Should().Be(400);
    }
}