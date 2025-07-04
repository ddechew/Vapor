namespace VaporWebAPI.Services.Interfaces;

/// <summary>
/// Defines a contract for sending emails asynchronously.
/// </summary>
public interface IEmailSender
{
    /// <summary>
    /// Sends an email asynchronously.
    /// </summary>
    /// <param name="toEmail">The recipient's email address.</param>
    /// <param name="subject">The subject of the email.</param>
    /// <param name="body">The HTML body content of the email.</param>
    /// <returns>A task that represents the asynchronous send operation.</returns>
    Task SendAsync(string toEmail, string subject, string body);
}
