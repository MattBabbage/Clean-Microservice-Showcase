namespace PostingService.UnitTests;
using MongoDB.Driver;
using EphemeralMongo;
using Moq;
using StatusDefinition;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Moq.Protected;
using System.Text.Json;

public partial class BaseUnitTest
{
    protected IDistributedCache MockRedisCache { get; set; }
    protected IConfiguration MockConfiguration {get; set;} 
    protected readonly Mock<IHttpClientFactory> MockHttpClientFactory;
    protected StatusLocation httpStatusLocation {get; set;}
    protected IMongoRunner MongoDBExecutor {get; set;}
    protected IMongoDatabase InMemoryDatabase {get;set;}
    protected IMongoCollection<Status> Collection {get; set;}
    
    public BaseUnitTest(){
        // Mock HTTP Request
        //Setup generic return from http request
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
            .ReturnsAsync(new HttpResponseMessage(){Content = httpContent});
        var httpClient = new HttpClient(handlerMock.Object) {
                BaseAddress = new Uri("https://ipgeolocation.abstractapi.com/v1/")
            };
        MockHttpClientFactory = new Mock<IHttpClientFactory>();
        MockHttpClientFactory.Setup(_ => _.CreateClient("Location")).Returns(httpClient);

        // Mock Redis Cache
        //Create InMemory Cache
        var opts = Options.Create(new MemoryDistributedCacheOptions());
        MockRedisCache = new MemoryDistributedCache(opts);
        
        // Mock Configuration
        //Setup Config - Needed for API Key 
        var myConfiguration = new Dictionary<string, string>
        {
            {"Location:ApiKey", "DoesntMatter!"}
        };
        MockConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(myConfiguration)
            .Build();

        //  MONGODB - In Memory
        //Setup MongoDB Options
        var options = new MongoRunnerOptions
        {
            MongoPort = 27017,
            //Ensure processes are cancelled on disposal
            KillMongoProcessesWhenCurrentProcessExits = true
        };
        //Create InMemory MongoDB Mocks
        MongoDBExecutor = MongoRunner.Run();
        InMemoryDatabase = new MongoClient(MongoDBExecutor.ConnectionString).GetDatabase("default");
        InMemoryDatabase.CreateCollection("InMemory_statuses");
        Collection = InMemoryDatabase.GetCollection<Status>("InMemory_statuses");

    }
}