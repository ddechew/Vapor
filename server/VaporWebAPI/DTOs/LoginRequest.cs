namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents the data required for a user to log in.
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// The username provided by the user attempting to log in.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The password associated with the user's account.
    /// </summary>
    public string Password { get; set; }
}
