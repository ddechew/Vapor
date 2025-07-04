namespace VaporWebAPI.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using VaporWebAPI.DTOs;
using VaporWebAPI.Services;

/// <summary>
/// Manages a user's wishlist, allowing adding, removing, reprioritizing, and retrieving wishlist apps.
/// </summary>
[ApiController]
[Route("api/wishlist")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly WishlistService wishlistService;

    public WishlistController(WishlistService wishlistService)
    {
        this.wishlistService = wishlistService;
    }

    /// <summary>
    /// Retrieves the authenticated user's wishlist, sorted by priority.
    /// </summary>
    /// <returns>List of wishlist apps with metadata.</returns>
    [HttpGet]
    public async Task<IActionResult> GetWishlist()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        var result = await wishlistService.GetWishlistAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Adds a new app to the user's wishlist. If the app already exists, the request will fail.
    /// </summary>
    /// <param name="dto">WishlistDTO containing AppId and optional priority.</param>
    /// <returns>200 OK if added, 409 Conflict if already exists.</returns>
    [HttpPost("add")]
    public async Task<IActionResult> AddToWishlist([FromBody] WishlistDTO dto)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        bool result = await wishlistService.AddToWishlistAsync(userId, dto.AppId, dto.Priority);
        return result ? Ok() : Conflict("Already in wishlist.");
    }

    /// <summary>
    /// Removes an app from the user's wishlist.
    /// </summary>
    /// <param name="appId">ID of the app to remove.</param>
    /// <returns>200 OK if removed, 404 Not Found if the app was not in the wishlist.</returns>
    [HttpDelete("remove/{appId}")]
    public async Task<IActionResult> Remove(int appId)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        bool result = await wishlistService.RemoveFromWishlistAsync(userId, appId);
        return result ? Ok() : NotFound();
    }

    /// <summary>
    /// Updates the priority of an app in the wishlist. Automatically shifts others accordingly.
    /// </summary>
    /// <param name="dto">WishlistDTO with AppId and new Priority.</param>
    /// <returns>200 OK if successful, 404 Not Found if the app was not in the wishlist.</returns>
    [HttpPut("priority")]
    public async Task<IActionResult> UpdatePriority([FromBody] WishlistDTO dto)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        bool result = await wishlistService.UpdatePriorityAsync(userId, dto.AppId, dto.Priority);
        return result ? Ok() : NotFound();
    }

    /// <summary>
    /// Normalizes wishlist priorities (1-based, sequential) after changes.
    /// </summary>
    /// <returns>200 OK after normalization.</returns>
    [HttpPut("normalize")]
    public async Task<IActionResult> NormalizeWishlist()
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
        await wishlistService.NormalizeWishlistPrioritiesAsync(userId);
        return Ok();
    }
}
