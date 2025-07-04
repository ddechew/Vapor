namespace VaporWebAPI.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using VaporWebAPI.Services;

/// <summary>
/// Manages user notifications, allowing retrieval, marking as read, and clearing all notifications.
/// Requires authentication.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificationController : ControllerBase
{
    private readonly NotificationService _notificationService;

    public NotificationController(NotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Retrieves all notifications for the currently authenticated user, ordered by creation time.
    /// </summary>
    /// <returns>List of user notifications.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllForUser()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var notifications = await _notificationService.GetAllForUserAsync(userId);
        return Ok(notifications);
    }

    /// <summary>
    /// Marks a specific notification as read (deletes it).
    /// </summary>
    /// <param name="id">The ID of the notification to mark as read.</param>
    [HttpPost("mark-as-read/{id}")]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        await _notificationService.MarkAsReadAsync(id);
        return NoContent();
    }

    /// <summary>
    /// Clears all notifications for the currently authenticated user.
    /// </summary>
    [HttpDelete("clear")]
    public async Task<IActionResult> ClearAll()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        await _notificationService.ClearAllAsync(userId);
        return NoContent();
    }
}
