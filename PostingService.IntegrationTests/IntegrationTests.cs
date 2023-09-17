namespace PostingService.IntegrationTests;
using MongoDB.Driver;
using StatusAPI;
using StatusDefinition;
using Microsoft.Extensions.Caching.Distributed;
using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;
public class IntegrationTests
{
    //Aim: Test the main functionality of the application using the whole system (CRUD)

    [Fact]
    public async Task CreateStatus()
    {
        // Arrange
        var payload = new StatusRequest(){Title="Test", Message="Test", IPAddress="2.103.61.107"};
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        // Act
        var result = await client.PostAsJsonAsync("/Status", payload);
        var content = await result.Content.ReadFromJsonAsync<Status>();
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(content);
    }


}