namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a single thumbnail in the YouTube API response.
/// </summary>
public class YouTubeThumbnail
{
    /// <summary>
    /// The URL of the thumbnail image.
    /// </summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }
}
