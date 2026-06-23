using CFFFusions.Models;

namespace CFFFusions.Services;

public interface IRefreshTokenService
{
    Task SaveAsync(RefreshToken refreshToken);

    Task<RefreshToken?> GetByTokenAsync(string token);

    Task UpdateAsync(RefreshToken refreshToken);

    Task RevokeAllByFamilyIdAsync(string familyId); // new
}