namespace PostingService.UnitTests;
using StatusAPI;
using MongoDB.Driver;
using FluentAssertions;
using StatusDefinition;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

public class GetLocationTests : BaseUnitTest
{
    [Fact]
    public async Task GetLocation_StandardAPICall()
    {
        // Arrange
        //No need to create data - testing data not found
        StatusRequest statusRequest = new StatusRequest(){
            IPAddress = "1.0.0.127",
            Title = "",
            Message = ""
        };
        // Act
        var location = await StatusAPI.GetLocation(statusRequest, MockRedisCache, MockHttpClientFactory.Object, MockConfiguration);
        // Assert
        // Tested method with data found aswell, works as expected
        location.Should().BeEquivalentTo(httpStatusLocation);
    }

    [Fact]
    public async Task GetLocation_IPCached()
    {
        var cachedLocation = new StatusLocation(){
                    City = "Cached",
                    Region = "Cached",
                    Country = "Cached"};
        var expectedData = JsonSerializer.SerializeToUtf8Bytes(cachedLocation);
        // Arrange
        //No need to create data - testing data not found
        MockRedisCache.Set("1.0.0.127", expectedData);
        StatusRequest statusRequest = new StatusRequest(){
            IPAddress = "1.0.0.127",
            Title = "",
            Message = ""
        };
        // Act
        var location = await StatusAPI.GetLocation(statusRequest, MockRedisCache, MockHttpClientFactory.Object, MockConfiguration);
        // Assert
        // Tested method with data found aswell, works as expected
        location.Should().BeEquivalentTo(cachedLocation);
    }
}