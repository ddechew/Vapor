namespace VaporWebAPI.Services;

using System.Net.Mail;
using System.Net;

using VaporWebAPI.Services.Interfaces;

public class SmtpEmailSenderService : IEmailSender
{
    private readonly IConfiguration _config;

    public SmtpEmailSenderService(IConfiguration config)
    {
        _config = config;
    }

    /// <summary>
    /// Sends HTML emails using SMTP settings defined in the application configuration.
    /// </summary>
    public async Task SendAsync(string toEmail, string subject, string body)
    {
        var smtpHost = _config["Smtp:Host"];
        var smtpPort = int.Parse(_config["Smtp:Port"]);
        var smtpUser = _config["Smtp:Username"];
        var smtpPass = _config["Smtp:Password"];

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPass),
            EnableSsl = true
        };

        var mail = new MailMessage
        {
            From = new MailAddress(smtpUser, "Vapor"),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };
        mail.To.Add(toEmail);

        await client.SendMailAsync(mail);
    }
}