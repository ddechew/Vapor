namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents the model used to populate a password reset email.
/// </summary>
public class ResetPasswordEmailModel
{
    /// <summary>
    /// The username of the user requesting the password reset.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The unique link that allows the user to reset their password.
    /// </summary>
    public string ResetLink { get; set; }
}
