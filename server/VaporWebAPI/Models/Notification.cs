namespace VaporWebAPI.Models;

/// <summary>
/// Represents a notification sent to a user.
/// Notifications can be linked to a post and optionally have a sender (another user).
/// </summary>
public partial class Notification
{
    /// <summary>
    /// Primary key of the notification.
    /// </summary>
    public int NotificationId { get; set; }

    /// <summary>
    /// The ID of the user who receives the notification.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Optional ID of the user who triggered the notification.
    /// </summary>
    public int? SenderId { get; set; }

    /// <summary>
    /// Message content of the notification.
    /// </summary>
    public string Message { get; set; } = null!;

    /// <summary>
    /// Date and time when the notification was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Optional ID of the related post for this notification.
    /// </summary>
    public int? PostId { get; set; }

    /// <summary>
    /// Timestamp of the last modification for auditing purposes.
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the related post, if any.
    /// </summary>
    public virtual Post? Post { get; set; }

    /// <summary>
    /// Navigation property to the user who receives the notification.
    /// </summary>
    public virtual User User { get; set; } = null!;
}
