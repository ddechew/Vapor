namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents the data required to render an email verification or account deletion template.
/// </summary>
public class EmailVerificationModel
{
    /// <summary>
    /// The username of the user to whom the email is being sent.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The URL the user can visit to verify their email address.
    /// </summary>
    public string VerificationUrl { get; set; }

    /// <summary>
    /// The URL the user can visit to delete their account.
    /// </summary>
    public string DeleteUrl { get; set; }


    /// <summary>
    /// Indicates whether the verification was triggered by Google authentication.
    /// </summary>
    public bool FromGoogle { get; set; } = false;
}
