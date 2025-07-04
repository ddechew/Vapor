namespace VaporWebAPI.Controllers;

using Microsoft.AspNetCore.Mvc;

using VaporWebAPI.DTOs;
using VaporWebAPI.Services;

/// <summary>
/// Provides endpoints for managing a user's shopping cart, including adding, removing, clearing, and merging cart items.
/// </summary>
[ApiController]
[Route("api/cart")]
public class CartController : ControllerBase
{
    private readonly CartService _cartService;

    public CartController(CartService cartService)
    {
        _cartService = cartService;
    }

    /// <summary>
    /// Retrieves all items in the specified user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>List of apps in the user's cart.</returns>
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCart(int userId)
    {
        var items = await _cartService.GetUserCartAsync(userId);
        return Ok(items);
    }

    /// <summary>
    /// Adds an app to the user's cart, checking for base game ownership if necessary.
    /// </summary>
    /// <param name="model">The user and app IDs.</param>
    [HttpPost("add")]
    public async Task<IActionResult> AddToCart([FromBody] CartRequest model)
    {
        await _cartService.AddToCartAsync(model.UserId, model.AppId);
        return Ok();
    }

    /// <summary>
    /// Removes a specific app from the user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="appId">The ID of the app to remove.</param>
    [HttpDelete("remove/{userId}/{appId}")]
    public async Task<IActionResult> RemoveFromCart(int userId, int appId)
    {
        await _cartService.RemoveFromCartAsync(userId, appId);
        return Ok();
    }

    /// <summary>
    /// Clears all items from the specified user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    [HttpDelete("clear/{userId}")]
    public async Task<IActionResult> ClearCart(int userId)
    {
        await _cartService.ClearCartAsync(userId);
        return Ok();
    }

    /// <summary>
    /// Merges a guest cart into the authenticated user's cart during login.
    /// </summary>
    /// <param name="model">The user ID and list of guest cart app IDs.</param>
    [HttpPost("merge")]
    public async Task<IActionResult> MergeGuestCart([FromBody] MergeCartRequest model)
    {
        await _cartService.MergeGuestCartAsync(model.UserId, model.GuestAppIds);
        return Ok();
    }
}
