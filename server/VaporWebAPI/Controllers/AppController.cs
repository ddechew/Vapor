namespace VaporWebAPI.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Security.Claims;

using VaporWebAPI.DTOs;
using VaporWebAPI.Services;

/// <summary>
/// Provides endpoints for retrieving app data, reviews, related content, and user-specific actions such as reviewing and ownership checks.
/// </summary>
[Route("api/apps")]
[ApiController]
public class AppController : ControllerBase
{
    private readonly AppService appService;

    public AppController(AppService appService)
    {
        this.appService = appService;
    }

    /// <summary>
    /// Retrieves the top 5 most purchased apps of type "game".
    /// </summary>
    [HttpGet("top5")]
    public async Task<IActionResult> GetTop5Apps()
    {
        var apps = await appService.GetTop5AppsAsync();
        return Ok(apps);
    }

    /// <summary>
    /// Retrieves a paginated list of game apps sorted by popularity.
    /// </summary>
    /// <param name="page">The page number (default is 1).</param>
    /// <param name="limit">The number of apps per page (default is 21).</param>
    [HttpGet]
    public async Task<IActionResult> GetPaginatedApps([FromQuery] int page = 1, [FromQuery] int limit = 21)
    {
        var apps = await appService.GetPaginatedAppsAsync(page, limit);
        return Ok(apps);
    }

    /// <summary>
    /// Retrieves detailed information for a specific app by its ID.
    /// </summary>
    /// <param name="id">The ID of the app.</param>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppDetails(int id)
    {
        var app = await appService.GetAppByIdAsync(id);
        if (app == null) return NotFound();
        return Ok(app);
    }

    /// <summary>
    /// Retrieves all related apps (DLCs, music, etc.) based on a base app ID.
    /// </summary>
    /// <param name="baseAppId">The base app ID to find related content for.</param>
    [HttpGet("related/{baseAppId}")]
    public async Task<IActionResult> GetRelatedApps(int baseAppId)
    {
        var relatedApps = await appService.GetRelatedAppsAsync(baseAppId);
        return Ok(relatedApps);
    }

    /// <summary>
    /// Retrieves a list of all available game genres.
    /// </summary>
    [HttpGet("genres")]
    public async Task<IActionResult> GetAllGenres()
    {
        var genres = await appService.GetAllGenresAsync();
        return Ok(genres);
    }

    /// <summary>
    /// Searches for apps based on name, genre, price range, and whether they are free.
    /// </summary>
    /// <param name="query">Search term (optional).</param>
    /// <param name="isFree">Whether to filter free or paid apps.</param>
    /// <param name="genres">List of genres to filter by.</param>
    /// <param name="priceSort">Sort direction ("asc" or "desc") for price.</param>
    [HttpGet("search")]
    public async Task<IActionResult> SearchApps(
        [FromQuery] string? query,
        [FromQuery] bool? isFree,
        [FromQuery] List<string>? genres,
        [FromQuery] string? priceSort)
    {
        var results = await appService.SearchAppsAsync(query, isFree, genres, priceSort);
        return Ok(results);
    }

    /// <summary>
    /// Retrieves all reviews for a given app.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    [HttpGet("{appId}/reviews")]
    public async Task<IActionResult> GetReviews(int appId)
    {
        var reviews = await appService.GetReviewsForAppAsync(appId);
        return Ok(reviews);
    }

    /// <summary>
    /// Adds a review for an app by the currently authenticated user.
    /// </summary>
    /// <param name="request">The review data.</param>
    [Authorize]
    [HttpPost("review")]
    public async Task<IActionResult> PostReview([FromBody] CreateReviewRequest request)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        var result = await appService.AddReviewAsync(userId, request);
        if (result == "Success")
            return Ok(new { message = "Review submitted." });

        return BadRequest(new { error = result });
    }

    /// <summary>
    /// Updates an existing review by the currently authenticated user.
    /// </summary>
    /// <param name="reviewId">The ID of the review to update.</param>
    /// <param name="request">The updated review content.</param>
    [Authorize]
    [HttpPut("review/{reviewId}")]
    public async Task<IActionResult> UpdateReview(int reviewId, [FromBody] CreateReviewRequest request)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var result = await appService.UpdateReviewAsync(userId, reviewId, request);

        if (result == "Updated")
            return Ok(new { message = "Review updated." });

        return BadRequest(new { error = result });
    }

    /// <summary>
    /// Checks if the current user has already submitted a review for the specified app.
    /// </summary>
    /// <param name="appId">The ID of the app to check.</param>
    [Authorize]
    [HttpGet("has-reviewed/{appId}")]
    public async Task<IActionResult> HasReviewed(int appId)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        bool hasReviewed = await appService.HasUserReviewedAppAsync(userId, appId);
        return Ok(hasReviewed);
    }

    /// <summary>
    /// Checks if the authenticated user owns the specified app.
    /// </summary>
    /// <param name="appId">The ID of the app to check.</param>
    [Authorize]
    [HttpGet("owns/{appId}")]
    public async Task<IActionResult> CheckOwnership(int appId)
    {
        int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        bool owns = await appService.UserOwnsAppAsync(userId, appId);
        return Ok(owns);
    }
}
