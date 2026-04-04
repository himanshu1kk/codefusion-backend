using Microsoft.AspNetCore.Mvc;
using CFFFusions.Services;

namespace CFFFusions.Controllers;

[ApiController]
[Route("api/handle-verification")]
public class HandleVerificationController : ControllerBase
{
    private readonly IHandleVerificationService _service;

    public HandleVerificationController(IHandleVerificationService service)
    {
        _service = service;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start(string userId, string handle)
    {
        await _service.StartVerificationAsync(userId, handle);

        return Ok(new
        {
            message = "OTP sent to your email"
        });
    }

    [HttpPost("check")]
    public async Task<IActionResult> Check(string userId, string otp)
    {
        var verified = await _service.VerifyAsync(userId, otp);

        return Ok(new
        {
            verified
        });
    }
}