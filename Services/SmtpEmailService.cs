// using CFFFusions.Models;
// using Microsoft.Extensions.Options;
// using System.Net;
// using System.Net.Mail;

// namespace CFFFusions.Services;

// public class SmtpEmailService : IEmailService
// {
//     private readonly SmtpSettings _settings;

//     public SmtpEmailService(IOptions<SmtpSettings> options)
//     {
//         _settings = options.Value;
//     }

//     public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
//     {
//            Console.WriteLine("email sent to us0" +_settings.Username );
//            Console.WriteLine("the user name is " +_settings.Username );
//         var message = new MailMessage
//         {
//             From = new MailAddress(_settings.Username, "CFFFusions"),
//             Subject = subject,
//             Body = htmlContent,
//             IsBodyHtml = true
//         };
//            Console.WriteLine("the message is " +message.From );

//                 Console.WriteLine("email sent to us1");

//         message.To.Add(toEmail);
//                 Console.WriteLine("email sent to us1");

//         using var smtp = new SmtpClient(_settings.Host, _settings.Port)
//         {
//             Credentials = new NetworkCredential(
//                 _settings.Username,
//                 _settings.Password
//             ),
//             EnableSsl = true
//         };
//         Console.WriteLine("email sent to us");

//         await smtp.SendMailAsync(message);
//     }
// }
