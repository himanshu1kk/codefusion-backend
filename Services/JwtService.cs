using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CFFFusions.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CFFFusions.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(IOptions<JwtSettings> options)
    {
        _settings = options.Value;
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserId),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("name", user.Name)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_settings.SecretKey)
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_settings.ExpiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
