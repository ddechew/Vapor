namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the release date information from the Steam app details API.
/// </summary>
public class AppDetailsReleaseDate
{
    /// <summary>
    /// Indicates whether the app is marked as "Coming Soon" on Steam.
    /// </summary>
    [JsonPropertyName("coming_soon")]
    public bool ComingSoon { get; set; }

    /// <summary>
    /// The release date string provided by Steam. Can be a full date, year, or "Coming Soon".
    /// </summary>
    [JsonPropertyName("date")]
    public string Date { get; set; }
}
