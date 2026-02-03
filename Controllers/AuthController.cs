using System.Security.Cryptography;
using System.Text;
using CFFFusions.Models;
using CFFFusions.Services;
using Microsoft.AspNetCore.Mvc;
using Cff.Error.Exceptions;
using Cff.Models;
using Cff.Error.Extensions;

namespace CFFFusions.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IJwtService _jwtService;

    public AuthController(
        IUserService userService,
        IJwtService jwtService)
    {
        _userService = userService;
        _jwtService = jwtService;
    }

    // ======================= LOGIN =======================

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var user = await _userService.GetByEmailAsync(request.Email);

            if (user == null)
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.USER_NOT_FOUND,
                        "User not found"
                    )
                );
            }

            if (!user.IsVerified)
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.BAD_REQUEST,
                        "Email not verified"
                    )
                );
            }

            var hashedPassword = HashPassword(request.Password);

            if (user.Password != hashedPassword)
            {
                throw new CffError(
                    new BaseResponse(
                        CffError.BAD_REQUEST,
                        "Invalid credentials"
                    )
                );
            }

            var token = _jwtService.GenerateToken(user);

            return Ok(new
            {
                accessToken = token,
                tokenType = "Bearer"
            });
        }
        catch (CffError err)
    {
        return err.ToActionResult();
    }
    }

    // ======================= HELPERS =======================

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        return Convert.ToBase64String(
            sha256.ComputeHash(Encoding.UTF8.GetBytes(password))
        );
    }
}
