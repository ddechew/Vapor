namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a video trailer or promotional video associated with a Steam app,
/// including links to MP4 formats and a thumbnail image.
/// </summary>
public class AppDetailsVideo
{
    /// <summary>
    /// A dictionary of MP4 video URLs, usually keyed by resolution (e.g., "480", "720").
    /// </summary>
    [JsonPropertyName("mp4")]
    public Dictionary<string, string> Mp4 { get; set; }

    /// <summary>
    /// URL to the thumbnail image representing the video.
    /// </summary>
    [JsonPropertyName("thumbnail")]
    public string Thumbnail { get; set; }
}
