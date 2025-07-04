namespace VaporWebAPI.DTOs;

/// <summary>
/// Data Transfer Object used for creating a new community post.
/// </summary>
public class CreatePostDTO
{
    /// <summary>
    /// The ID of the app associated with the post.
    /// </summary>
    public int AppId { get; set; }


    /// <summary>
    /// The textual content of the post.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Optional URL of an image attached to the post.
    /// </summary>
    public string? ImageUrl { get; set; }
}
