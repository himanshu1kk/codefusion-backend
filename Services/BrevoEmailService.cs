using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CFFFusions.Models;
using Microsoft.Extensions.Options;

namespace CFFFusions.Services;

public class BrevoEmailService : IEmailService
{
    private readonly HttpClient _httpClient;
    private readonly BrevoSettings _settings;

    public BrevoEmailService(
        HttpClient httpClient,
        IOptions<BrevoSettings> options)
    {
        _httpClient = httpClient;
        _settings = options.Value;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
    {
        if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            throw new Exception("Brevo API key is missing");

        var payload = new
        {
            sender = new
            {
                email = _settings.SenderEmail,
                name = _settings.SenderName
            },
            to = new[]
            {
                new { email = toEmail }
            },
            subject = subject,
            htmlContent = htmlContent
        };

        var request = new HttpRequestMessage(
            HttpMethod.Post,
            "https://api.brevo.com/v3/smtp/email"
        );

        request.Headers.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json")
        );

        request.Headers.Add("api-key", _settings.ApiKey);

        request.Content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json"
        );

        Console.WriteLine("Sending email via Brevo API...");

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Brevo email failed: {response.StatusCode} - {error}");
        }

        Console.WriteLine("Email sent successfully via Brevo.");
    }
}
