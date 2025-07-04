namespace VaporWebAPI.Controllers;

using Microsoft.AspNetCore.Mvc;

using VaporWebAPI.Services;

/// <summary>
/// Handles importing cleaned app data into the system.
/// </summary>
[Route("api/[controller]")]
[ApiController]
public class AppImportController : ControllerBase
{
    private readonly AppImportService appImportService;

    public AppImportController(AppImportService appImportService)
    {
        this.appImportService = appImportService;
    }

    /// <summary>
    /// Imports applications from a pre-cleaned JSON file into the database.
    /// Skips already existing or invalid apps.
    /// </summary>
    /// <returns>A success message or an error if the cleaned file is missing or empty.</returns>
    [HttpPost("import-apps")]
    public async Task<IActionResult> ImportApps()
    {
        var result = await appImportService.ImportAppsAsync();
        return Ok(result);
    }
}
