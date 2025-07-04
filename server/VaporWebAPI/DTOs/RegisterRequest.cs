namespace VaporWebAPI.DTOs;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Represents the data required to register a new user in the system.
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// The desired username of the user.
    /// Must be unique across the platform.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The display name that will be shown publicly.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// The email address of the user.
    /// Must be in valid email format.
    /// </summary>
    [EmailAddress]
    public string Email { get; set; }

    /// <summary>
    /// The password chosen by the user for account access.
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Indicates whether the user should be registered with admin privileges.
    /// </summary>
    public bool IsAdmin { get; set; }
}
