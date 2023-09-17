namespace PostingService.UnitTests;
using StatusAPI;
using MongoDB.Driver;
using EphemeralMongo;
using FluentAssertions;
using Moq;
using StatusDefinition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Configuration;
using Moq.Protected;
using System.Text.Json;
public class CreateStatusTests : BaseUnitTest
{
    [Fact]
    public void CreateStatus_Standard()
    {
        // Arrange
        //No need to create data - testing data not found
        StatusRequest statusRequest = new StatusRequest(){
            IPAddress = "1.0.0.127",
            Title = "",
            Message = ""
        };
        // Act
        var creationResponse = StatusAPI.CreateStatus(statusRequest, MockHttpClientFactory.Object, Collection, MockRedisCache, MockConfiguration).Result as Ok<Status>;
        // Assert
        // Tested method with data found aswell, works as expected
        creationResponse.StatusCode.Should().Be(200);
        creationResponse.Value.Title.Should().Be(statusRequest.Title);
    }

    [Fact]
    public void CreateStatus_BadIp()
    {
        StatusRequest statusRequest = new StatusRequest(){
            IPAddress = "NotAnIp",
            Title = "",
            Message = ""
        };
        // Act
        var creationResponse = StatusAPI.CreateStatus(statusRequest, MockHttpClientFactory.Object, Collection, MockRedisCache, MockConfiguration).Result as BadRequest<string>;
        // Assert
        // Tested method with data found aswell, works as expected
        creationResponse.StatusCode.Should().Be(400);
    }
}