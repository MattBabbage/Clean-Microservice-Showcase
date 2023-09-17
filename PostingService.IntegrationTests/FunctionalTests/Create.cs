namespace PostingService.IntegrationTests;
using StatusDefinition;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;

public class CreateTests
{
    //Aim: Test the main functionality of the application using the whole system (CRUD)

    [Fact]
    public async Task CreateStatus_IPv4()
    {
        // Arrange
        var payload = new StatusRequest(){Title="Test", Message="Test", IPAddress="2.103.61.107"};
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        // Act
        var result = await client.PostAsJsonAsync("/Status", payload);
        var content = await result.Content.ReadFromJsonAsync<Status>();
        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
    }

    
    [Fact]
    public async Task CreateStatus_IPv6()
    {
        // Arrange
        var payload = new StatusRequest(){Title="Test", Message="Test", IPAddress="2a02:c7c:88cd:8000:9175:ad0:83ce:eced"};
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        // Act
        var result = await client.PostAsJsonAsync("/Status", payload);
        var content = await result.Content.ReadFromJsonAsync<Status>();
        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.OK);
        content.Should().NotBeNull();
    }

    [Fact]
    public async Task CreateStatus_IncorrectIp()
    {
        // Arrange
        var payload = new StatusRequest(){Title="Test", Message="Test", IPAddress=":)"};
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        // Act
        var result = await client.PostAsJsonAsync("/Status", payload);
        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
    
}