namespace VaporWebAPI.Models;

/// <summary>
/// Represents a user-generated post related to a game (App).
/// Posts can contain content, optional images, and are linked to a user and an app.
/// </summary>
public partial class Post
{
    /// <summary>
    /// Primary key identifier for the post.
    /// </summary>
    public int PostId { get; set; }

    /// <summary>
    /// ID of the user who created the post. Nullable to support deleted or anonymous posts.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Display name of the user at the time of posting.
    /// </summary>
    public string UserDisplayName { get; set; } = null!;

    /// <summary>
    /// The textual content of the post.
    /// </summary>
    public string Content { get; set; } = null!;

    /// <summary>
    /// Timestamp when the post was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The ID of the app (game) that this post is related to.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Optional URL to an image attached to the post.
    /// </summary>
    public string? ImageUrl { get; set; }

    /// <summary>
    /// Timestamp of the last modification to this post.
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the related App entity.
    /// </summary>
    public virtual App App { get; set; } = null!;

    /// <summary>
    /// Collection of notifications related to this post (e.g., likes, comments).
    /// </summary>
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    /// <summary>
    /// Collection of comments posted under this post.
    /// </summary>
    public virtual ICollection<PostComment> PostComments { get; set; } = new List<PostComment>();

    /// <summary>
    /// Collection of likes associated with this post.
    /// </summary>
    public virtual ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();

    /// <summary>
    /// Navigation property to the User entity who created the post.
    /// Nullable for cases where the user is deleted or unknown.
    /// </summary>
    public virtual User? User { get; set; }
}
