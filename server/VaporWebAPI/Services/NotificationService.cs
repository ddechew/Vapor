namespace VaporWebAPI.Services;

using Microsoft.EntityFrameworkCore;

using VaporWebAPI.Data;
using VaporWebAPI.Models;

public class NotificationService
{
    private readonly VaporDbContext _context;

    public NotificationService(VaporDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all notifications for a specific user, ordered by newest first.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>A list of notifications for the user.</returns>
    public async Task<List<Notification>> GetAllForUserAsync(int userId)
    {
        return await _context.Notifications
            .Where(n => n.UserId == userId)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new notification for a user, optionally linked to a post and sender.
    /// </summary>
    /// <param name="userId">The ID of the recipient user.</param>
    /// <param name="senderId">The ID of the sender (optional).</param>
    /// <param name="message">The message content of the notification.</param>
    /// <param name="postId">The related post ID, if any.</param>
    public async Task CreateNotificationAsync(int userId, int? senderId, string message, int? postId = null)
    {
        var notification = new Notification
        {
            UserId = userId,
            SenderId = senderId,
            Message = message,
            CreatedAt = DateTime.UtcNow,
            PostId = postId
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Marks a specific notification as read by removing it from the database.
    /// </summary>
    /// <param name="notificationId">The ID of the notification to remove.</param>
    public async Task MarkAsReadAsync(int notificationId)
    {
        var notification = await _context.Notifications.FindAsync(notificationId);
        if (notification == null) return;

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Clears all notifications for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user whose notifications will be removed.</param>
    public async Task ClearAllAsync(int userId)
    {
        var notifications = _context.Notifications.Where(n => n.UserId == userId);
        _context.Notifications.RemoveRange(notifications);
        await _context.SaveChangesAsync();
    }
}
