namespace VaporWebAPI.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using VaporWebAPI.DTOs.AdminDTOs;
using VaporWebAPI.Models;
using VaporWebAPI.Services;

/// <summary>
/// Provides administrative endpoints for managing users, roles, and developers.
/// Requires authentication and admin role authorization.
/// </summary>
[Authorize(Roles = "Admin")] 
[Route("api/admin")]
[ApiController]
public class AdminController : Controller
{
    private readonly AdminService _adminService;
    public AdminController(AdminService adminService)
    {
        _adminService = adminService;
    }

    //===================================================================================================================================
    // == USER ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves a list of all users with their role and account details.
    /// </summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _adminService.GetAllUsersAsync();
        return Ok(users);
    }

    /// <summary>
    /// Exports all user data to an Excel file.
    /// </summary>
    [HttpGet("users/export")]
    public async Task<IActionResult> ExportUsersExcel()
    {
        var fileBytes = await _adminService.GenerateUsersExcelAsync();
        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "users.xlsx");
    }

    /// <summary>
    /// Updates an existing user's profile, role, or wallet information.
    /// </summary>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="updatedUser">The updated user data.</param>
    [HttpPut("users/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserAdminDTO updatedUser)
    {
        if (id != updatedUser.UserId)
            return BadRequest(new { message = "User ID mismatch." });

        var actorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var success = await _adminService.UpdateUserAsync(actorId, updatedUser);
        if (!success) return NotFound(new { message = "User not found." });

        return Ok(new { message = "User updated successfully." });
    }

    /// <summary>
    /// Creates a new user account with a role, wallet, and password.
    /// </summary>
    /// <param name="newUser">The new user to create.</param>
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser([FromBody] UserAdminDTO newUser)
    {
        try
        {
            var actorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var created = await _adminService.CreateUserAsync(actorId, newUser);
            return Ok(created); 
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Deletes a user account unless it's the current admin themselves.
    /// </summary>
    /// <param name="id">The ID of the user to delete.</param>
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var actorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(actorId, out var parsedActorId))
            return Unauthorized();

        if (parsedActorId == id)
            return BadRequest(new { message = "You cannot delete your own account." });

        var success = await _adminService.DeleteUserAsync(parsedActorId, id);
        if (!success)
            return NotFound(new { message = "User not found or already deleted." });

        return Ok(new { message = "User deleted successfully." });
    }

    //===================================================================================================================================
    // == ROLES ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all roles in the system.
    /// </summary>
    [HttpGet("roles")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _adminService.GetAllRolesAsync();
        return Ok(roles);
    }

    /// <summary>
    /// Creates a new role (e.g., Admin, User).
    /// </summary>
    /// <param name="newRole">The role data to create.</param>
    [HttpPost("roles")]
    public async Task<IActionResult> CreateRole([FromBody] Role newRole)
    {
        if (string.IsNullOrWhiteSpace(newRole.RoleName))
            return BadRequest(new { message = "Role name cannot be empty." });

        var actorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var created = await _adminService.CreateRoleAsync(actorId, newRole);
        return Ok(created);
    }

    /// <summary>
    /// Updates the name of an existing role.
    /// </summary>
    /// <param name="id">The ID of the role to update.</param>
    /// <param name="updatedRole">The updated role data.</param>
    [HttpPut("roles/{id}")]
    public async Task<IActionResult> UpdateRole(int id, [FromBody] Role updatedRole)
    {
        var actorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var success = await _adminService.UpdateRoleAsync(actorId, id, updatedRole);
        if (!success)
            return NotFound(new { message = "Role not found." });

        return Ok(new { message = "Role updated successfully." });
    }

    /// <summary>
    /// Deletes an existing role by ID.
    /// </summary>
    /// <param name="id">The ID of the role to delete.</param>
    [HttpDelete("roles/{id}")]
    public async Task<IActionResult> DeleteRole(int id)
    {
        var success = await _adminService.DeleteRoleAsync(id);
        if (!success)
            return NotFound(new { message = "Role not found." });

        return Ok();
    }

    /// <summary>
    /// Exports all roles to an Excel file.
    /// </summary>
    [HttpGet("roles/export")]
    public async Task<IActionResult> ExportRolesExcel()
    {
        var fileBytes = await _adminService.GenerateRolesExcelAsync();
        return File(fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "roles.xlsx");
    }

    //===================================================================================================================================
    // == DEVELOPERS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all developers in the system.
    /// </summary>
    [HttpGet("developers")]
    public async Task<IActionResult> GetAllDevelopers()
    {
        var devs = await _adminService.GetAllDevelopersAsync();
        return Ok(devs);
    }

    /// <summary>
    /// Creates a new developer entry.
    /// </summary>
    /// <param name="dev">The developer data to add.</param>
    [HttpPost("developers")]
    public async Task<IActionResult> CreateDeveloper([FromBody] Developer dev)
    {
        if (string.IsNullOrWhiteSpace(dev.DeveloperName))
            return BadRequest(new { message = "Developer name is required." });

        var created = await _adminService.CreateDeveloperAsync(dev);
        return Ok(created);
    }

    /// <summary>
    /// Updates an existing developer's name.
    /// </summary>
    /// <param name="id">The ID of the developer to update.</param>
    /// <param name="dev">The updated developer data.</param>
    [HttpPut("developers/{id}")]
    public async Task<IActionResult> UpdateDeveloper(int id, [FromBody] Developer dev)
    {
        var success = await _adminService.UpdateDeveloperAsync(id, dev);
        if (!success) return NotFound(new { message = "Developer not found." });

        return Ok();
    }

    /// <summary>
    /// Deletes a developer by ID.
    /// </summary>
    /// <param name="id">The ID of the developer to delete.</param>
    [HttpDelete("developers/{id}")]
    public async Task<IActionResult> DeleteDeveloper(int id)
    {
        var success = await _adminService.DeleteDeveloperAsync(id);
        if (!success) return NotFound(new { message = "Developer not found." });

        return Ok();
    }

    /// <summary>
    /// Exports all developers to an Excel file.
    /// </summary>
    [HttpGet("developers/export")]
    public async Task<IActionResult> ExportDevelopersExcel()
    {
        var file = await _adminService.GenerateDevelopersExcelAsync();
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "developers.xlsx");
    }

    //===================================================================================================================================
    // == APP TYPES ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all app types.
    /// </summary>
    [HttpGet("apptypes")]
    public async Task<IActionResult> GetAllAppTypes()
    {
        var types = await _adminService.GetAllAppTypesAsync();
        return Ok(types);
    }

    /// <summary>
    /// Creates a new app type.
    /// </summary>
    /// <param name="newType">The app type to create.</param>
    [HttpPost("apptypes")]
    public async Task<IActionResult> CreateAppType([FromBody] AppType newType)
    {
        if (string.IsNullOrWhiteSpace(newType.TypeName))
            return BadRequest(new { message = "Type name is required." });

        var created = await _adminService.CreateAppTypeAsync(newType);
        return Ok(created);
    }

    /// <summary>
    /// Updates an existing app type.
    /// </summary>
    /// <param name="id">The id of the app type to update.</param>
    /// <param name="updated">The updated app type data.</param>
    [HttpPut("apptypes/{id}")]
    public async Task<IActionResult> UpdateAppType(int id, [FromBody] AppType updated)
    {
        var success = await _adminService.UpdateAppTypeAsync(id, updated);
        if (!success) return NotFound(new { message = "Type not found." });

        return Ok();
    }

    /// <summary>
    /// Deletes an app type.
    /// </summary>
    /// <param name="id">The id of the app type to delete.</param>
    [HttpDelete("apptypes/{id}")]
    public async Task<IActionResult> DeleteAppType(int id)
    {
        var success = await _adminService.DeleteAppTypeAsync(id);
        if (!success) return NotFound(new { message = "Type not found." });

        return Ok();
    }

    //===================================================================================================================================
    // == APPS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all applications.
    /// </summary>
    /// <returns>List of all apps.</returns>
    [HttpGet("apps")]
    public async Task<IActionResult> GetAllApps()
    {
        var apps = await _adminService.GetAllAppsAsync();
        return Ok(apps);
    }

    /// <summary>
    /// Creates a new application.
    /// </summary>
    /// <param name="dto">The app creation data transfer object.</param>
    /// <returns>The created app or a bad request with error message.</returns>
    [HttpPost("apps")]
    public async Task<IActionResult> CreateApp([FromBody] AppCreateDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.AppName))
            return BadRequest(new { message = "App name is required." });

        try
        {
            var created = await _adminService.CreateAppAsync(dto);
            return Ok(created);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Updates an existing application.
    /// </summary>
    /// <param name="id">The ID of the app to update.</param>
    /// <param name="dto">The app data transfer object with updated values.</param>
    /// <returns>The updated app or not found/bad request results.</returns>
    [HttpPut("apps/{id}")]
    public async Task<IActionResult> UpdateApp(int id, [FromBody] AppAdminDTO dto)
    {
        if (id != dto.AppId)
            return BadRequest(new { message = "App ID mismatch." });

        var result = await _adminService.UpdateAppAsync(id, dto);
        if (result == null)
            return NotFound(new { message = "App not found." });

        return Ok(result);
    }

    /// <summary>
    /// Deletes an application by ID.
    /// </summary>
    /// <param name="id">The ID of the app to delete.</param>
    /// <returns>No content if deleted; NotFound otherwise.</returns>
    [HttpDelete("apps/{id}")]
    public async Task<IActionResult> DeleteApp(int id)
    {
        var success = await _adminService.DeleteAppAsync(id);
        return success ? NoContent() : NotFound(new { message = "App not found." });
    }

    //===================================================================================================================================
    // == GENRES ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all genres.
    /// </summary>
    /// <returns>List of genres as GenreAdminDTO.</returns>
    [HttpGet("genres")]
    public async Task<ActionResult<IEnumerable<GenreAdminDTO>>> GetGenres()
    {
        var genres = await _adminService.GetAllGenresAsync();
        return Ok(genres);
    }

    //===================================================================================================================================
    // == PUBLISHERS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all publishers.
    /// </summary>
    /// <returns>List of publishers as PublisherAdminDTO.</returns>
    [HttpGet("publishers")]
    public async Task<ActionResult<IEnumerable<PublisherAdminDTO>>> GetPublishers()
    {
        var result = await _adminService.GetPublishersAsync();
        return Ok(result);
    }

    //===================================================================================================================================
    // == POSTS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all posts.
    /// </summary>
    /// <returns>List of posts as PostAdminDTO.</returns>
    [HttpGet("posts")]
    public async Task<ActionResult<List<PostAdminDTO>>> GetAllPosts()
    {
        var posts = await _adminService.GetAllPostsAsync();
        return Ok(posts);
    }

    //===================================================================================================================================
    // == POST COMMENTS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all comments on posts.
    /// </summary>
    /// <returns>List of comments as CommentAdminDTO.</returns>
    [HttpGet("comments")]
    public async Task<ActionResult<List<CommentAdminDTO>>> GetAllComments()
    {
        var comments = await _adminService.GetAllCommentsAsync();
        return Ok(comments);
    }

    //===================================================================================================================================
    // == POST LIKES ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all post likes.
    /// </summary>
    /// <returns>List of post likes as PostLikeAdminDTO.</returns>
    [HttpGet("postlikes")]
    public async Task<ActionResult<List<PostLikeAdminDTO>>> GetAllPostLikes()
    {
        var likes = await _adminService.GetAllPostLikesAsync();
        return Ok(likes);
    }

    //===================================================================================================================================
    // == APP REVIEWS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all application reviews.
    /// </summary>
    /// <returns>List of app reviews as AppReviewAdminDTO.</returns>
    [HttpGet("appreviews")]
    public async Task<ActionResult<List<AppReviewAdminDTO>>> GetAllAppReviews()
    {
        var reviews = await _adminService.GetAllAppReviewsAsync();
        return Ok(reviews);
    }

    //===================================================================================================================================
    // == APP IMAGES ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all app images.
    /// </summary>
    /// <returns>List of app images as AppImageAdminDTO.</returns>
    [HttpGet("appimages")]
    public async Task<ActionResult<List<AppImageAdminDTO>>> GetAllAppImages()
    {
        var images = await _adminService.GetAllAppImagesAsync();
        return Ok(images);
    }

    //===================================================================================================================================
    // == APP IMAGES ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all app videos.
    /// </summary>
    /// <returns>List of app videos as AppVideoAdminDTO.</returns>
    [HttpGet("appvideos")]
    public async Task<ActionResult<List<AppVideoAdminDTO>>> GetAllAppVideos()
    {
        var videos = await _adminService.GetAllAppVideosAsync();
        return Ok(videos);
    }

    //===================================================================================================================================
    // == CART ITEMS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all cart items.
    /// </summary>
    /// <returns>List of cart items as CartItemAdminDTO.</returns>
    [HttpGet("cartitems")]
    public async Task<ActionResult<List<CartItemAdminDTO>>> GetAllCartItems()
    {
        var items = await _adminService.GetAllCartItemsAsync();
        return Ok(items);
    }

    //===================================================================================================================================
    // == WISHLIST ITEMS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all wishlist items.
    /// </summary>
    /// <returns>List of wishlist items as WishlistAdminDTO.</returns>
    [HttpGet("wishlist")]
    public async Task<ActionResult<List<WishlistAdminDTO>>> GetWishlistItems()
    {
        var wishlist = await _adminService.GetAllWishlistItemsAsync();
        return Ok(wishlist);
    }

    //===================================================================================================================================
    // == PURCHASE HISTORY ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves the purchase history.
    /// </summary>
    /// <returns>List of purchase history records as PurchaseHistoryAdminDTO.</returns>
    [HttpGet("purchase-history")]
    public async Task<ActionResult<List<PurchaseHistoryAdminDTO>>> GetPurchaseHistory()
    {
        var history = await _adminService.GetPurchaseHistoryAsync();
        return Ok(history);
    }

    //===================================================================================================================================
    // == NOTIFICATIONS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all notifications.
    /// </summary>
    /// <returns>List of notifications as NotificationAdminDTO.</returns>
    [HttpGet("notifications")]
    public async Task<ActionResult<List<NotificationAdminDTO>>> GetAllNotifications()
    {
        var notifications = await _adminService.GetAllNotificationsAsync();
        return Ok(notifications);
    }

    //===================================================================================================================================
    // == USER LIBRARIES ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all user libraries.
    /// </summary>
    /// <returns>List of user libraries as UserLibraryAdminDTO.</returns>
    [HttpGet("libraries")]
    public async Task<ActionResult<List<UserLibraryAdminDTO>>> GetUserLibraries()
    {
        var result = await _adminService.GetUserLibrariesAsync();
        return Ok(result);
    }
}
