namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents a notification sent to a user.
/// </summary>
public class NotificationDTO
{
    /// <summary>
    /// The ID of the user who is receiving the notification.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The ID of the user who triggered the notification (optional).
    /// </summary>
    public int? SenderId { get; set; }

    /// <summary>
    /// The content of the notification message.
    /// </summary>
    public string Message { get; set; }
}
