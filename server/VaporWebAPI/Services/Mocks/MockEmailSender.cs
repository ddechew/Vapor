namespace VaporWebAPI.Services.Mocks;

using VaporWebAPI.Services.Interfaces;

using System.Diagnostics;

/// <summary>
/// A mock implementation of <see cref="IEmailSender"/> used for testing purposes.
/// Instead of sending real emails, it outputs the email content to the debug console.
/// </summary>
public class MockEmailSender : IEmailSender
{
    /// <summary>
    /// Simulates sending an email by writing the details to the debug output.
    /// </summary>
    /// <param name="toEmail">The recipient's email address.</param>
    /// <param name="subject">The subject line of the email.</param>
    /// <param name="body">The body content of the email.</param>
    /// <returns>A completed task.</returns>
    public Task SendAsync(string toEmail, string subject, string body)
    {
        Debug.WriteLine($"--- Mock Email Sent ---");
        Debug.WriteLine($"To: {toEmail}");
        Debug.WriteLine($"Subject: {subject}");
        Debug.WriteLine($"Body: {body}");
        Debug.WriteLine($"------------------------");
        return Task.CompletedTask;
    }
}
