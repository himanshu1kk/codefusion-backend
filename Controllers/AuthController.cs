using CFFFusions.Models;
using CFFFusions.Services;
using Cff.Error.Exceptions;
using Cff.Error.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace CFFFusions.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        try
        {
            var result =
                await _authService.LoginAsync(request);

            AppendRefreshCookie(
                result.RefreshToken,
                result.RefreshTokenExpiry);

            return Ok(new
            {
                accessToken = result.AccessToken,
                tokenType = result.TokenType
            });
        }
        catch (CffError err)
        {
            return err.ToActionResult();
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        try
        {
            var refreshToken =
                Request.Cookies["refreshToken"];

            var result =
                await _authService
                .RefreshAsync(refreshToken);

            AppendRefreshCookie(
                result.RefreshToken,
                result.RefreshTokenExpiry);

            return Ok(new
            {
                accessToken = result.AccessToken,
                tokenType = result.TokenType
            });
        }
        catch (CffError err)
        {
            return err.ToActionResult();
        }
    }

    private void AppendRefreshCookie(
        string token,
        DateTime expiry)
    {
        Response.Cookies.Append(
            "refreshToken",
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                 SameSite = SameSiteMode.None,
                Expires = expiry
            });
    }
}