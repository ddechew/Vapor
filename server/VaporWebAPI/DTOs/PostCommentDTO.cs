namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents a comment on a post, including user info and content details.
/// </summary>
public class PostCommentDTO
{
    /// <summary>
    /// The unique identifier of the comment.
    /// </summary>
    public int CommentId { get; set; }

    /// <summary>
    /// The username of the user who made the comment.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The display name of the user who made the comment.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The URL of the user's profile picture.
    /// </summary>
    public string? ProfilePicture { get; set; }

    /// <summary>
    /// The content of the comment.
    /// </summary>
    public string CommentText { get; set; } = string.Empty;

    /// <summary>
    /// The date and time when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indicates whether the comment was edited after being posted.
    /// </summary>
    public bool IsEdited { get; set; }
}
