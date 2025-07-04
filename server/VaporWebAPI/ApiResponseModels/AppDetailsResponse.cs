namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents detailed information about a Steam app retrieved from the Steam Store API.
/// </summary>
public class AppDetailsResponse
{
    /// <summary>
    /// Unique Steam application ID.
    /// </summary>
    [JsonPropertyName("steam_appid")]
    public int AppId { get; set; }

    /// <summary>
    /// Name of the application.
    /// </summary>
    [JsonPropertyName("name")]
    public string AppName { get; set; }

    /// <summary>
    /// Type of the application (e.g., "game", "dlc", "demo").
    /// </summary>
    [JsonPropertyName("type")]
    public string AppType { get; set; }

    /// <summary>
    /// Release date information.
    /// </summary>
    [JsonPropertyName("release_date")]
    public AppDetailsReleaseDate ReleaseDate { get; set; }

    /// <summary>
    /// Indicates whether the app is free to play.
    /// </summary>
    [JsonPropertyName("is_free")]
    public bool IsFree { get; set; }

    /// <summary>
    /// Short description provided by the developer or publisher.
    /// </summary>
    [JsonPropertyName("short_description")]
    public string Description { get; set; }

    /// <summary>
    /// Price details if the app is not free (null for free apps).
    /// </summary>
    [JsonPropertyName("price_overview")]
    public AppDetailsPriceOverview? PriceOverview { get; set; }

    /// <summary>
    /// If the app is additional content (e.g. DLC), points to the base game.
    /// </summary>
    [JsonPropertyName("fullgame")]
    public AppDetailsFullGame? FullGame { get; set; }

    /// <summary>
    /// List of developer names.
    /// </summary>
    [JsonPropertyName("developers")]
    public List<string> Developers { get; set; }

    /// <summary>
    /// List of publisher names.
    /// </summary>
    [JsonPropertyName("publishers")]
    public List<string> Publishers { get; set; }

    /// <summary>
    /// List of genres assigned to the app.
    /// </summary>
    [JsonPropertyName("genres")]
    public List<AppDetailsGenre> Genres { get; set; }

    /// <summary>
    /// URL to the main header image.
    /// </summary>
    [JsonPropertyName("header_image")]
    public string HeaderImage { get; set; }

    /// <summary>
    /// List of screenshot image objects.
    /// </summary>
    [JsonPropertyName("screenshots")]
    public List<AppDetailsScreenshot> Screenshots { get; set; }

    /// <summary>
    /// List of video trailers or preview clips.
    /// </summary>
    [JsonPropertyName("movies")]
    public List<AppDetailsVideo> Videos { get; set; }
}
