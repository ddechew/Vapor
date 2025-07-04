namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents a user who has liked a post, including basic identity information.
/// </summary>
public class UserLikeDTO
{
    /// <summary>
    /// The unique username of the user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the user, used for showing in UI.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The URL to the user's profile picture (if available).
    /// </summary>
    public string? ProfilePicture { get; set; }
}
