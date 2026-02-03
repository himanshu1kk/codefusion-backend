using CFFFusions.Models;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace CFFFusions.Services;

public class SmtpEmailService : IEmailService
{
    private readonly SmtpSettings _settings;

    public SmtpEmailService(IOptions<SmtpSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        var message = new MailMessage
        {
            From = new MailAddress(_settings.Username, "CFFFusions"),
            Subject = subject,
            Body = htmlContent,
            IsBodyHtml = true
        };

        message.To.Add(toEmail);

        using var smtp = new SmtpClient(_settings.Host, _settings.Port)
        {
            Credentials = new NetworkCredential(
                _settings.Username,
                _settings.Password
            ),
            EnableSsl = true
        };

        await smtp.SendMailAsync(message);
    }
}
