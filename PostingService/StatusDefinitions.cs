using System.Net;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace StatusDefinition
{
    public record StatusRequest
    {
        public required string IPAddress { get; set; }
        public required string Title { get; set; }
        public required string Message { get; set; }
    };

    public class Status
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; } 
        public StatusLocation? Location { get; set; }
    }
    
    public class StatusLocation
    {
        //Serialization from snake case, which comes back from api call,
        //is only supported by 3rd party lib which can be avoided easily (hence [JsonPropertyName(snake_case_equivilant)])
        [JsonPropertyName("city")]
        public string City { get; set; } = null!;
        [JsonPropertyName("region")]
        public string Region { get; set; } = null!;
        [JsonPropertyName("country")]
        public string Country { get; set; } = null!;
    }
}