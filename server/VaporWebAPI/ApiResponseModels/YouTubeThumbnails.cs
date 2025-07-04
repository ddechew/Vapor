namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the collection of thumbnail images with different resolutions
/// returned in a YouTube API video snippet.
/// </summary>
public class YouTubeThumbnails
{
    /// <summary>
    /// The default resolution thumbnail (typically 120x90).
    /// </summary>
    [JsonPropertyName("default")]
    public YouTubeThumbnail Default { get; set; }

    /// <summary>
    /// The medium resolution thumbnail (typically 320x180).
    /// </summary>
    [JsonPropertyName("medium")]
    public YouTubeThumbnail Medium { get; set; }

    /// <summary>
    /// The high resolution thumbnail (typically 480x360).
    /// </summary>
    [JsonPropertyName("high")]
    public YouTubeThumbnail High { get; set; }
}
