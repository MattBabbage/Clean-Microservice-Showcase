namespace PostingService.IntegrationTests;
using StatusDefinition;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http.Json;
using Xunit;
using FluentAssertions;

public class DeleteTests
{
    //Aim: Test the main functionality of the application using the whole system (CRUD)

    [Fact]
    public async Task DeleteStatus_Exists()
    {
        // Arrange
        //Create a new status
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var payload = new StatusRequest(){Title="ForDeletion", Message="", IPAddress="2.103.61.107"};

        // Act
        //Create Data to read
        var createdResult = await client.PostAsJsonAsync("/Status", payload);
        var createdContent = await createdResult.Content.ReadFromJsonAsync<Status>();
        string CreatedDataId = createdContent.Id;

        //Update
        var result = await client.DeleteAsync("/Status/" + CreatedDataId);

        // Assert
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
    }

    [Fact]
    public async Task DeleteStatus_DoesntExist()
    {
        // Arrange
        //Create a new status
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        var payload = new StatusRequest(){Title="ForDeletion", Message="", IPAddress="2.103.61.107"};

        // Act
        //Create Data to read
        var createdResult = await client.PostAsJsonAsync("/Status", payload);
        var createdContent = await createdResult.Content.ReadFromJsonAsync<Status>();
        string CreatedDataId = createdContent.Id;

        //Update
        var result = await client.DeleteAsync("/Status/"+"FakeId");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task DeleteStatus_NotFound()
    {
        // Arrange
        //Create a new status
        await using var application = new WebApplicationFactory<Program>();
        using var client = application.CreateClient();
        // Act
        //Assuming 650724524066b23143635afe does not exist - very unlikely
        var result = await client.DeleteAsync("/Status/650724524066b23143635afe");

        // Assert
        result.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}