using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using CFFFusions.Services;

namespace CFFFusions.Controllers;

[ApiController]
[Route("api/handle-verification")]
[Authorize] 
public class HandleVerificationController : ControllerBase
{
    private readonly IHandleVerificationService _service;

    public HandleVerificationController(IHandleVerificationService service)
    {
        _service = service;
    }

    
    private string GetUserId()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                     ?? User.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userId))
        {
            throw new Exception("Invalid token: userId not found");
        }

        return userId;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromQuery] string handle)
    {
        var userId = GetUserId();

        await _service.StartVerificationAsync(userId, handle);

        return Ok(new
        {
            message = "OTP sent to your email"
        });
    }

    [HttpPost("check")]
    public async Task<IActionResult> Check([FromQuery] string otp)
    {
        var userId = GetUserId();

        var message = await _service.VerifyAsync(userId, otp);

        return Ok(new
        {
            message
        });
    }
}