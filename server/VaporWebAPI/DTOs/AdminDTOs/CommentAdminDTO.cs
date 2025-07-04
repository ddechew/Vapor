namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object for representing a comment in the admin panel.
/// </summary>
public class CommentAdminDTO
{
    /// <summary>
    /// Unique identifier of the comment.
    /// </summary>
    public int CommentId { get; set; }

    /// <summary>
    /// Identifier of the post to which the comment belongs.
    /// </summary>
    public int PostId { get; set; }

    /// <summary>
    /// Content of the post being commented on.
    /// </summary>
    public string PostContent { get; set; }

    /// <summary>
    /// Identifier of the user who posted the comment (nullable if user deleted).
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Display name of the user who posted the comment.
    /// </summary>
    public string UserDisplayName { get; set; }

    /// <summary>
    /// Text content of the comment.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Timestamp when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
