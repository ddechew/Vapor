namespace VaporWebAPI.Models;

/// <summary>
/// Represents a "like" given by a user to a post.
/// </summary>
public partial class PostLike
{
    /// <summary>
    /// Primary key identifier for the like record.
    /// </summary>
    public int LikeId { get; set; }

    /// <summary>
    /// Foreign key to the post that was liked.
    /// </summary>
    public int PostId { get; set; }

    /// <summary>
    /// Foreign key to the user who liked the post. Nullable if user is deleted or anonymous.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Timestamp of the last modification for this record.
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the related Post.
    /// </summary>
    public virtual Post Post { get; set; } = null!;

    /// <summary>
    /// Navigation property to the User who liked the post.
    /// Nullable for deleted or anonymous users.
    /// </summary>
    public virtual User? User { get; set; }
}
