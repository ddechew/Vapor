namespace VaporWebAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

using VaporWebAPI.Services;

/// <summary>
/// Handles interactions with the YouTube API to retrieve and assign trailers to apps.
/// </summary>
[Route("api/youtube")]
[ApiController]
public class YouTubeController : ControllerBase
{
    private readonly YouTubeService youtubeService;

    public YouTubeController(YouTubeService youtubeService)
    {
        this.youtubeService = youtubeService;
    }

    /// <summary>
    /// Retrieves a YouTube trailer and thumbnail for the given game name.
    /// </summary>
    /// <param name="gameName">The name of the game to search for on YouTube.</param>
    /// <returns>
    /// 200 OK with trailer and thumbnail URLs if found;  
    /// 404 Not Found if no trailer is available.
    /// </returns>
    [HttpGet("trailer/{gameName}")]
    public async Task<IActionResult> GetGameTrailer(string gameName)
    {
        var (trailerUrl, thumbnailUrl) = await youtubeService.GetYouTubeTrailerAsync(gameName);
        return (trailerUrl != null && thumbnailUrl != null) ? Ok(new { trailer = trailerUrl, thumbnail = thumbnailUrl  }) : NotFound("Trailer not found"); 
    }

    /// <summary>
    /// Updates all apps of type 'game' that are missing trailers by fetching YouTube trailers.
    /// </summary>
    /// <returns>
    /// 200 OK with a message indicating how many apps were updated.
    /// </returns>
    [HttpPost("update-apps-with-trailers")]
    public async Task<IActionResult> UpdateAppsWithYouTubeTrailers()
    {
        var updatedCount = await youtubeService.UpdateAppsWithYouTubeTrailersAsync();
        return Ok($"{updatedCount} apps were updated with YouTube trailers.");
    }
}
