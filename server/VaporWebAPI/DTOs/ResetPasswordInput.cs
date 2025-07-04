namespace VaporWebAPI.DTOs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the input required to reset a user's password.
/// </summary>
public class ResetPasswordInput
{
    /// <summary>
    /// The token provided to authorize the password reset.
    /// </summary>
    [Required]
    public string Token { get; set; } = null!;

    /// <summary>
    /// The new password the user wants to set.
    /// </summary>
    [Required]
    public string NewPassword { get; set; } = null!;
}
