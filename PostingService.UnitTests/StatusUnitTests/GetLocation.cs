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
public class GetLocationTests
{
    private IDistributedCache MockRedisCache { get; set; }
    private IConfiguration MockConfiguration {get; set;} 
    private readonly Mock<IHttpClientFactory> MockHttpClientFactory;
    
    private StatusLocation httpStatusLocation {get; set;}

    public GetLocationTests(){
        //Setup generic return
        httpStatusLocation = new StatusLocation(){
                    City = "Test",
                    Region = "Test",
                    Country = "Test"};
        StringContent httpContent = new StringContent(JsonSerializer.Serialize(httpStatusLocation), System.Text.Encoding.UTF8, "application/json");
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock.Protected()
    .Setup<Task<HttpResponseMessage>>(
        "SendAsync",
        ItExpr.IsAny<HttpRequestMessage>(),
        ItExpr.IsAny<CancellationToken>()
    )
    .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
    {
        HttpResponseMessage response = new HttpResponseMessage(){Content = httpContent};

        // configure your response here

        return response;
    });
        var httpClient = new HttpClient(handlerMock.Object) {
                BaseAddress = new Uri("https://ipgeolocation.abstractapi.com/v1/")
            };
        MockHttpClientFactory = new Mock<IHttpClientFactory>();
        MockHttpClientFactory.Setup(_ => _.CreateClient("Location")).Returns(httpClient);


        var opts = Options.Create(new MemoryDistributedCacheOptions());
        MockRedisCache = new MemoryDistributedCache(opts);
        //Setup Config - Needed for API Key 
        var myConfiguration = new Dictionary<string, string>
        {
            {"Location:ApiKey", "DoesntMatter!"}
        };
        MockConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();
    }

    [Fact]
    public void GetLocation_StandardAPICall()
    {
        // Arrange
        //No need to create data - testing data not found
        StatusRequest statusRequest = new StatusRequest(){
            IPAddress = "",
            Title = "",
            Message = ""
        };
        // Act
        var location = StatusAPI.GetLocation(statusRequest, MockRedisCache, MockHttpClientFactory.Object, MockConfiguration).Result;
        // Assert
        // Tested method with data found aswell, works as expected
        location.Should().BeEquivalentTo(httpStatusLocation);
    }

    [Fact]
    public void GetLocation_IPCached()
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
        var location = StatusAPI.GetLocation(statusRequest, MockRedisCache, MockHttpClientFactory.Object, MockConfiguration).Result;
        // Assert
        // Tested method with data found aswell, works as expected
        location.Should().BeEquivalentTo(cachedLocation);
    }
}