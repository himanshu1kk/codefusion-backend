using System.Text.Json.Serialization;

namespace CFFFusions.Models;

public class User
{
    [JsonPropertyName("userId")]
    public Guid UserId { get; private set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = null!;

    [JsonPropertyName("email")]
    public string Email { get; set; } = null!;

    [JsonPropertyName("password")]
    public string Password { get; set; } = null!;

    [JsonPropertyName("isVerified")]
    public bool IsVerified { get; set; } = false;

    [JsonPropertyName("isRegistered")]
    public bool IsRegistered { get; set; } = false;


     [JsonPropertyName("cfUserName")]
    public string CfUserName { get; set; } = null!;


  
    public User()
    {
        UserId = Guid.NewGuid();
    }
}
