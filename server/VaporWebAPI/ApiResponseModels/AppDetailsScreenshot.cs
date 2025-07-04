namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a screenshot associated with a Steam app, including both full-size and thumbnail image URLs.
/// </summary>
public class AppDetailsScreenshot
{
    /// <summary>
    /// URL to the thumbnail version of the screenshot.
    /// </summary>
    [JsonPropertyName("path_thumbnail")]
    public string Thumbnail { get; set; }

    /// <summary>
    /// URL to the full-size version of the screenshot.
    /// </summary>
    [JsonPropertyName("path_full")]
    public string FullImage { get; set; }
}
