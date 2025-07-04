namespace VaporWebAPI.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using VaporWebAPI.DTOs;
using VaporWebAPI.Services;

/// <summary>
/// Manages user-related operations including profile updates, account changes, purchases, and library access.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Retrieves user information by ID. Only accessible by authenticated users.
    /// </summary>
    /// <param name="id">User ID to retrieve.</param>
    /// <returns>User basic profile details.</returns>
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _userService.GetUserDetailsAsync(id);
        if (user == null) return NotFound();

        return Ok(new
        {
            user.UserId,
            user.Username,
            user.DisplayName,
            user.ProfilePicture,
            user.Wallet,
            user.Points
        });
    }

    /// <summary>
    /// Retrieves public user profile data by username.
    /// </summary>
    /// <param name="username">Username to look up.</param>
    /// <returns>Public profile details (username, display name, avatar).</returns>
    [HttpGet("by-username/{username}")]
    public async Task<IActionResult> GetUserByUsername(string username)
    {
        var user = await _userService.GetUserByUsernameAsync(username);
        if (user == null) return NotFound();

        return Ok(new
        {
            user.UserId,
            user.Username,
            user.DisplayName,
            user.ProfilePicture
        });
    }

    /// <summary>
    /// Updates the authenticated user's display name and profile picture.
    /// </summary>
    /// <param name="request">UpdateProfileRequest DTO.</param>
    /// <returns>Status message.</returns>
    [Authorize]
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        if (string.IsNullOrEmpty(username))
            return Unauthorized();


        var result = await _userService.UpdateUserProfileAsync(username, request);
        if (result == "User not found")
            return NotFound(result);


        return result == "Success"
            ? Ok(new { message = "Profile updated." })
            : BadRequest(new { message = result });
    }

    /// <summary>
    /// Starts the email change flow by sending a confirmation link to the new email.
    /// </summary>
    /// <param name="request">Contains new email address.</param>
    /// <returns>Status message.</returns>
    [Authorize]
    [HttpPost("initiate-email-change")]
    public async Task<IActionResult> InitiateEmailChange([FromBody] ChangeEmailRequest request)
    {
        var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        var result = await _userService.InitiateEmailChangeAsync(username!, request.NewEmail);

        return result == "Confirmation email sent."
            ? Ok(new { message = result })
            : BadRequest(new { message = result });
    }

    /// <summary>
    /// Confirms a previously requested email change using a token.
    /// </summary>
    /// <param name="token">Email change verification token.</param>
    /// <returns>Status message.</returns>
    [AllowAnonymous]
    [HttpPost("confirm-email-change")]
    public async Task<IActionResult> ConfirmEmailChange([FromQuery] string token)
    {
        var result = await _userService.ChangeEmailAsync(token);

        return result == "Success"
            ? Ok(new { message = "Email has been updated successfully." })
            : BadRequest(new { message = result });
    }

    /// <summary>
    /// Changes the current user's password after validating the old one.
    /// </summary>
    /// <param name="request">Contains current and new password.</param>
    /// <returns>Status message.</returns>
    [Authorize]
    [HttpPut("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest("Invalid password input.");

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await _userService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);

        return result == "Success"
            ? Ok(new { message = "Password changed successfully." })
            : BadRequest(new { message = result });
    }

    /// <summary>
    /// Changes the username of the authenticated user.
    /// </summary>
    /// <param name="newUsername">New username string.</param>
    /// <returns>Status message.</returns>
    [Authorize]
    [HttpPut("change-username")]
    public async Task<IActionResult> ChangeUsername([FromBody] string newUsername)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _userService.ChangeUsernameAsync(userId, newUsername);

        return result == "Success"
            ? Ok(new { message = "Username changed successfully." })
            : BadRequest(new { message = result });
    }

    /// <summary>
    /// Unlinks the Google account from the current user (requires password to be set).
    /// </summary>
    /// <returns>Status message.</returns>
    [Authorize]
    [HttpPost("unlink-google")]
    public async Task<IActionResult> UnlinkGoogle()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _userService.UnlinkGoogleAccountAsync(userId);

        return result == "Success"
            ? Ok(new { message = "Google account unlinked successfully." })
            : BadRequest(new { message = result });
    }

    /// <summary>
    /// Completes a wallet-based purchase, optionally applying point-based discounts.
    /// </summary>
    /// <param name="pointsToUse">Points to use for discount (optional).</param>
    /// <returns>Status message.</returns>
    [Authorize]
    [HttpPost("wallet/purchase")]
    public async Task<IActionResult> PurchaseWithWallet([FromQuery] int pointsToUse = 0)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        var result = await _userService.PurchaseWithWalletAsync(userId, pointsToUse);

        if (result == "Success")
            return Ok(new { message = "Purchase completed using wallet!" });

        return BadRequest(new { message = result });
    }

    /// <summary>
    /// Adds a free app to the user’s library (only if the app is free and base game is owned).
    /// </summary>
    /// <param name="appId">App ID to add.</param>
    /// <returns>Status message.</returns>
    [Authorize]
    [HttpPost("add-free/{appId}")]
    public async Task<IActionResult> AddFreeAppToLibrary(int appId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var result = await _userService.AddFreeAppToLibraryAsync(userId, appId);

        if (result == "Success")
            return Ok(new { message = "App added to library." });

        return BadRequest(new { message = result });
    }

    /// <summary>
    /// Retrieves the logged-in user’s game library, including related content (DLCs, music).
    /// </summary>
    /// <returns>List of owned applications and their related apps.</returns>
    [Authorize]
    [HttpGet("library")]
    public async Task<IActionResult> GetLibrary()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var library = await _userService.GetUserLibraryAsync(userId);
        return Ok(library);
    }

    /// <summary>
    /// Retrieves another user's public game library by their username.
    /// </summary>
    /// <param name="username">The target username.</param>
    /// <returns>List of owned base games and their related content.</returns>
    [HttpGet("library/{username}")]
    public async Task<IActionResult> GetUserLibraryByUsername(string username)
    {
        var user = await _userService.GetUserByUsernameAsync(username);
        if (user == null) return NotFound();

        var library = await _userService.GetUserLibraryAsync(user.UserId);
        return Ok(library);
    }

    /// <summary>
    /// Returns a history of the user's past purchases (including free apps and wallet top-ups).
    /// </summary>
    /// <returns>List of purchases with metadata.</returns>
    [Authorize]
    [HttpGet("purchase-history")]
    public async Task<IActionResult> GetPurchaseHistory()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        var history = await _userService.GetUserPurchaseHistoryAsync(userId);
        return Ok(history);
    }

    /// <summary>
    /// Uploads and sets a new profile picture for the current user.
    /// </summary>
    /// <param name="file">The image file (form-data).</param>
    /// <returns>The uploaded image URL or error.</returns>
    [Authorize]
    [HttpPost("upload-profile-picture")]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file)
    {
        var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        var result = await _userService.UploadProfilePictureAsync(username, file);

        if (result == null)
            return BadRequest("Failed to upload image.");

        return Ok(new { imageUrl = result });
    }
}
