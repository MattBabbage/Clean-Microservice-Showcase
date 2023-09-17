namespace PostingService.IntegrationTests;
using StatusDefinition;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;

public class UpdateTests
{
    [Fact]
    public async Task UpdateStatus()
    {
        // Arrange
        //Create a new status
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var payload = new StatusRequest(){Title="PreUpdate", Message="", IPAddress="2.103.61.107"};

        // Act
        //Create Data to read
        var createdResult = await client.PostAsJsonAsync("/Status", payload);
        var createdContent = await createdResult.Content.ReadFromJsonAsync<Status>();
        string CreatedDataId = createdContent.Id;
        //Change some of the data
        createdContent.Title = "PostUpdate";
        //Update
        var result = await client.PutAsJsonAsync("/Status/"+CreatedDataId, createdContent);
        var content = await result.Content.ReadFromJsonAsync<Status>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.NotNull(content);
        Assert.Equal(content.Title, "PostUpdate");
    }
}