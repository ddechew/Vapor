namespace VaporWebAPI.Models;

/// <summary>
/// Represents a comment made by a user on a specific post.
/// </summary>
public partial class PostComment
{
    /// <summary>
    /// Primary key identifier for the comment.
    /// </summary>
    public int CommentId { get; set; }

    /// <summary>
    /// Foreign key to the post that this comment belongs to.
    /// </summary>
    public int PostId { get; set; }

    /// <summary>
    /// Foreign key to the user who made the comment. Nullable if user is deleted or anonymous.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Display name of the user at the time the comment was made.
    /// </summary>
    public string UserDisplayName { get; set; } = null!;

    /// <summary>
    /// The textual content of the comment.
    /// </summary>
    public string CommentText { get; set; } = null!;

    /// <summary>
    /// Timestamp when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Indicates whether the comment was edited after creation.
    /// </summary>
    public bool IsEdited { get; set; }

    /// <summary>
    /// Timestamp of the last modification of the comment.
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the related Post.
    /// </summary>
    public virtual Post Post { get; set; } = null!;

    /// <summary>
    /// Navigation property to the User who made the comment.
    /// Nullable for deleted or anonymous users.
    /// </summary>
    public virtual User? User { get; set; }
}
