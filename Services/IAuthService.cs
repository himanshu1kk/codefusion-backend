using CFFFusions.Models;

namespace CFFFusions.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);

    Task<LoginResponse> RefreshAsync(string? refreshToken);
}