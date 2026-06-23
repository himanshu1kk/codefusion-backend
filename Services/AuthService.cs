using System.Security.Cryptography;
using System.Text;
using CFFFusions.Models;
using Cff.Error.Exceptions;
using Cff.Models;

namespace CFFFusions.Services;

public class AuthService : IAuthService
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;
    private readonly IRefreshTokenService _refreshTokenService;

    public AuthService(
        IUserService userService,
        IJwtService jwtService,
        IRefreshTokenService refreshTokenService)
    {
        _userService = userService;
        _jwtService = jwtService;
        _refreshTokenService = refreshTokenService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            var user =
                await _userService.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.USER_NOT_FOUND,
                        "Invalid Credentials"
                    )
                );
            }

            if (!user.IsVerified)
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.BAD_REQUEST,
                        "Invalid Credentials"
                    )
                );
            }

            var hashedPassword =
                HashPassword(request.Password);

            if (user.Password != hashedPassword)
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.BAD_REQUEST,
                        "Invalid Credentials"
                    )
                );
            }

            var accessToken =
                _jwtService.GenerateToken(user);

            var refreshToken =
                _jwtService.GenerateRefreshToken();

            var refreshTokenModel =
                new RefreshToken
                {
                    UserId = user.UserId,

                    Token = refreshToken,

                    FamilyId = Guid.NewGuid().ToString(),

                    IsRevoked = false,

                    CreatedAt = DateTime.UtcNow,

                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                };

            await _refreshTokenService.SaveAsync(
                refreshTokenModel
            );

            return new LoginResponse
            {
                AccessToken = accessToken,

                RefreshToken = refreshToken,

                RefreshTokenExpiry =
                    refreshTokenModel.ExpiresAt
            };
        }
        catch (CffError)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CffError(
                new BaseResponse(
                    CffError.INTERNAL_ERROR,
                    "Error during login"
                ),
                ex: ex
            );
        }
    }

    public async Task<LoginResponse> RefreshAsync(
        string? refreshToken)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.UNAUTHORIZED,
                        "Refresh token missing"
                    )
                );
            }

            var storedToken =
                await _refreshTokenService
                .GetByTokenAsync(refreshToken);

            if (storedToken == null)
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.UNAUTHORIZED,
                        "Invalid refresh token"
                    )
                );
            }

            if (storedToken.IsRevoked)
            {
                await _refreshTokenService
                    .RevokeAllByFamilyIdAsync(
                        storedToken.FamilyId
                    );

                throw new CffError(
                    new BaseResponse(
                        CffError.UNAUTHORIZED,
                        "Token reuse detected. Please login again."
                    )
                );
            }

            if (storedToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.UNAUTHORIZED,
                        "Refresh token expired"
                    )
                );
            }

            var user =
                await _userService
                .GetByIdAsync(storedToken.UserId);

            if (user == null)
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.USER_NOT_FOUND,
                        "User not found"
                    )
                );
            }

            var accessToken =
                _jwtService.GenerateToken(user);

            var newRefreshToken =
                _jwtService.GenerateRefreshToken();

            storedToken.IsRevoked = true;

            await _refreshTokenService
                .UpdateAsync(storedToken);

            var newToken =
                new RefreshToken
                {
                    UserId = user.UserId,

                    Token = newRefreshToken,

                    FamilyId = storedToken.FamilyId,

                    IsRevoked = false,

                    CreatedAt = DateTime.UtcNow,

                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                };

            await _refreshTokenService.SaveAsync(
                newToken
            );

            return new LoginResponse
            {
                AccessToken = accessToken,

                RefreshToken = newRefreshToken,

                RefreshTokenExpiry =
                    newToken.ExpiresAt
            };
        }
        catch (CffError)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new CffError(
                new BaseResponse(
                    CffError.INTERNAL_ERROR,
                    "Error refreshing token"
                ),
                ex: ex
            );
        }
    }

    private static string HashPassword(
        string password)
    {
        using var sha256 = SHA256.Create();

        return Convert.ToBase64String(
            sha256.ComputeHash(
                Encoding.UTF8.GetBytes(password)
            )
        );
    }
}