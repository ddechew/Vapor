namespace VaporWebAPI.DTOs;

/// <summary>
/// Data Transfer Object used to request a password change for an authenticated user.
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// The user's current password. Required to authorize the password change.
    /// </summary>
    public string CurrentPassword { get; set; }

    /// <summary>
    /// The new password that the user wants to set.
    /// </summary>
    public string NewPassword { get; set; }
}
