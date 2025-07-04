namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object representing a post for admin views.
/// </summary>
public class PostAdminDTO
{
    /// <summary>
    /// Unique identifier of the post.
    /// </summary>
    public int PostId { get; set; }

    /// <summary>
    /// Identifier of the related app.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Name of the related app.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// Identifier of the user who created the post (nullable).
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Display name of the user who created the post.
    /// </summary>
    public string UserDisplayName { get; set; }

    /// <summary>
    /// Content of the post.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Date and time when the post was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Number of likes the post has received.
    /// </summary>
    public int LikesCount { get; set; }

    /// <summary>
    /// Number of comments on the post.
    /// </summary>
    public int CommentsCount { get; set; }
}
