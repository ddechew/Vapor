namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the ID of a YouTube video returned by the YouTube Data API search response.
/// </summary>
public class YouTubeVideoId
{
    /// <summary>
    /// The unique identifier of the YouTube video.
    /// </summary>
    [JsonPropertyName("videoId")]
    public string VideoId { get; set; }
}
