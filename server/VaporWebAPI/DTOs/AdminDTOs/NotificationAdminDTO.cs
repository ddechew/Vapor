namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object representing a notification in the admin panel.
/// </summary>
public class NotificationAdminDTO
{
    /// <summary>
    /// Unique identifier of the notification.
    /// </summary>
    public int NotificationId { get; set; }

    /// <summary>
    /// Identifier of the user who received the notification (nullable).
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Display name of the user who received the notification (nullable).
    /// </summary>
    public string? UserDisplayName { get; set; }

    /// <summary>
    /// Notification message content.
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// Timestamp of when the notification was created or last modified.
    /// </summary>
    public DateTime Timestamp { get; set; }
}
