namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents a request to update a user's profile information.
/// </summary>
public class UpdateProfileRequest
{
    /// <summary>
    /// The new display name of the user. Optional.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// The new URL or path to the user's profile picture. Optional.
    /// </summary>
    public string? ProfilePicture { get; set; }
}
