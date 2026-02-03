using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace CFFFusions.Models;

public class User
{
    [BsonId]
    [JsonPropertyName("userId")]
    public string UserId { get; private set; }

    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public bool IsVerified { get; set; }

    public User()
    {
        UserId = Guid.NewGuid().ToString();
    }
}
