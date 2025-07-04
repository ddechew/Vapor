namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents the data required to log a user out by invalidating their refresh token.
/// </summary>
public class LogoutRequest
{
    /// <summary>
    /// The refresh token that should be invalidated during logout.
    /// </summary>
    public string RefreshToken { get; set; }
}
