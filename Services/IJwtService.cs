using CFFFusions.Models;

namespace CFFFusions.Services;

public interface IJwtService
{
    string GenerateToken(User user);
    string GenerateRefreshToken();
    
}
