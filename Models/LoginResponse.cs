namespace CFFFusions.Models;

public class LoginResponse
{
    public string AccessToken { get; set; } = string.Empty;

    public string RefreshToken { get; set; } = string.Empty;

    public DateTime RefreshTokenExpiry { get; set; }

    public string TokenType { get; set; } = "Bearer";
}