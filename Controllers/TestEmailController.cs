using CFFFusions.Services;
using Microsoft.AspNetCore.Mvc;

namespace CFFFusions.Controllers;

[ApiController]
[Route("api/test-email")]
public class TestEmailController : ControllerBase
{
    private readonly IEmailService _emailService;

    public TestEmailController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromQuery] string email)
    {
        Console.WriteLine("📧 Sending test email via Gmail SMTP...");

        await _emailService.SendEmailAsync(
            email,
            "✅ Email service is working",
            "<h2>Success 🎉</h2><p>Your Gmail SMTP email integration works.</p>"
        );

        return Ok("Email sent successfully");
    }
}
