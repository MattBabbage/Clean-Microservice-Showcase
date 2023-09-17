namespace PostingService.IntegrationTests;
using StatusDefinition;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;

public class ReadTests
{
    //Aim: Test the main functionality of the application using the whole system (CRUD)
    [Fact]
    public async Task ReadStatus()
    {
        // Arrange
        //Create a new status
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var payload = new StatusRequest(){Title="Test", Message="Test", IPAddress="2.103.61.107"};

        // Act
        //Create Data to read
        var createdResult = await client.PostAsJsonAsync("/Status", payload);
        var createdContent = await createdResult.Content.ReadFromJsonAsync<Status>();
        string CreatedDataId = createdContent.Id;

        var result = await client.GetAsync("/Status/"+CreatedDataId);
        var content = await result.Content.ReadFromJsonAsync<Status>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(content.Id, CreatedDataId);
    }
    
    [Fact]
    public async Task ReadStatuses()
    {
        // Arrange
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();

        // Act
        var result = await client.GetAsync("/Statuses?page=1&pageSize=5");
        var content = await result.Content.ReadFromJsonAsync<List<Status>>();
        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(content);
    }
}