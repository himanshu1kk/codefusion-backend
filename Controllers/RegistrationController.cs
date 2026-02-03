using Cff.Error.Exceptions;
using Cff.Error.Extensions;
using CFFFusions.Models;
using CFFFusions.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace CFFFusions.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class RegistrationController : ControllerBase
{
      private readonly IRegistrationService _registrationService;

    public RegistrationController(IRegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpPost("register")]
public async Task<IActionResult> Register(RegisterRequest request)
{
    try
    {
        var result = await _registrationService.RegisterAsync(request);
        return Ok(new { message = result });
    }
     catch (CffError err)
    {
        return err.ToActionResult();
    }
}

    [HttpPost("verify-otp")]
    public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpRequest request)
    {
        try
        {
            var message = await _registrationService.VerifyOtpAsync(request);
            return Ok(new { message });
        }
        catch (CffError err)
    {
        return err.ToActionResult();
    }
    }

}