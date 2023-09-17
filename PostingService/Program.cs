using StatusAPI;
using MongoDB.Driver;
using StatusDefinition;
using Microsoft.OpenApi.Models;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Adding Swagger support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Posting Status API",
        Description = "This is an API for posting to a travel based message board",
        TermsOfService = new Uri("https://example.com/terms"),
    });
    options.EnableAnnotations();
});

//Adding very basic rate limiting 
builder.Services.AddRateLimiter(_ => _
    .AddFixedWindowLimiter(policyName: "fixed", options =>
    {
        options.PermitLimit = 30;
        options.Window = TimeSpan.FromSeconds(10);
        options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        options.QueueLimit = 2; //Maximum cumulative permit count of queued acquisition requests.
    }));

// Setup our HTTPClients using IHttpClientFactory Pattern - Best Practice as of 17/09/23
// https://learn.microsoft.com/en-us/aspnet/core/fundamentals/http-requests?view=aspnetcore-7.0
builder.Services.AddHttpClient("Location", httpClient =>
{
    httpClient.BaseAddress = new Uri("https://ipgeolocation.abstractapi.com/v1");
});

//Build Redis Cache
builder.Services.AddStackExchangeRedisCache(options =>
 {
     options.Configuration = builder.Configuration.GetSection("Redis")["ConnectionString"];
     options.InstanceName = "IPCache";
 });

//Setup MongoDB Client Singletons
builder.Services.AddSingleton(_ => new MongoClient());
builder.Services.AddSingleton(provider => provider.GetRequiredService<MongoClient>().GetDatabase(builder.Configuration.GetSection("MongoDB")["DatabaseName"]));
builder.Services.AddSingleton(provider => provider.GetRequiredService<IMongoDatabase>().GetCollection<Status>(builder.Configuration.GetSection("MongoDB")["CollectionName"]));

//Adding Output Caching - Not super necessary but good feature for people refreshing page a lot
builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("OutputCacheDemonstrator" ,p => p.Expire(TimeSpan.FromSeconds(30)));
});

var app = builder.Build();

// Development Swagger check
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseOutputCache();

app.UseHttpsRedirection();
app.UseRateLimiter();

//Configuring custom endpoints from static class StatusAPI
app.ConfigureAPI();

app.Run();