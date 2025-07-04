namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the snippet part of a YouTube search result, containing metadata about the video.
/// </summary>
public class YouTubeSnippet
{
    /// <summary>
    /// The thumbnails associated with the YouTube video.
    /// </summary>
    [JsonPropertyName("thumbnails")]
    public YouTubeThumbnails Thumbnails { get; set; }
}
