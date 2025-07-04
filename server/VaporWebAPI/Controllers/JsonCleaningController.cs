namespace VaporWebAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
using VaporWebAPI.Services;

/// <summary>
/// Provides an endpoint to clean raw Steam app JSON data by filtering out invalid or inappropriate entries.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class JsonCleaningController : ControllerBase
{
    private readonly JsonCleaningService jsonCleaningService;

    public JsonCleaningController(JsonCleaningService jsonCleaningService)
    {
        this.jsonCleaningService = jsonCleaningService;
    }

    /// <summary>
    /// Cleans the original JSON file by removing apps with invalid names or banned content,
    /// and saves the result to a new cleaned JSON file.
    /// </summary>
    /// <returns>A message describing how many entries were removed and the location of the cleaned file.</returns>
    [HttpPost("clean")]
    public IActionResult CleanJson()
    {
        string result = jsonCleaningService.CleanJsonFile();
        return Ok(result);
    }
}
