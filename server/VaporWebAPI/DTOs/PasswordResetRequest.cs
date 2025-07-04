namespace VaporWebAPI.DTOs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents a request to initiate a password reset via email.
/// </summary>
public class PasswordResetRequest
{
    /// <summary>
    /// The email address of the user requesting a password reset.
    /// </summary>
    [Required, EmailAddress]
    public string Email { get; set; } = null!;
}
    