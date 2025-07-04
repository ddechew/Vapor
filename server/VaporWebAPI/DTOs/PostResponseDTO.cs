namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents the data returned for a post in the community section,
/// including content, metadata, user info, and related app information.
/// </summary>
public class PostResponseDTO
{
    /// <summary>
    /// The unique identifier of the post.
    /// </summary>
    public int PostId { get; set; }

    /// <summary>
    /// The textual content of the post.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// The date and time when the post was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Optional URL of the image attached to the post.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// The display name of the user who created the post.
    /// </summary>
    public string UserDisplayName { get; set; }

    /// <summary>
    /// The username of the user who created the post.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    /// The URL of the user's avatar.
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// The ID of the app the post is related to.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// The name of the app the post is about.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// The URL of the app's header image.
    /// </summary>
    public string? HeaderImage { get; set; }

    /// <summary>
    /// The number of likes the post has received.
    /// </summary>
    public int LikesCount { get; set; }

    /// <summary>
    /// The number of comments made on the post.
    /// </summary>
    public int CommentsCount { get; set; }
}
