namespace VaporWebAPI.DTOs;

/// <summary>
/// Data Transfer Object used for submitting a new comment on a post.
/// </summary>
public class CommentRequestDTO
{
    /// <summary>
    /// The ID of the post to which the comment is associated.
    /// </summary>
    public int PostId { get; set; }

    /// <summary>
    /// The textual content of the comment.
    /// </summary>
    public string CommentText { get; set; } = string.Empty;
}
