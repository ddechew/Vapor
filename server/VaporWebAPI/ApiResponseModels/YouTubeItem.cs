namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a single video item returned in a YouTube API search response.
/// </summary>
public class YouTubeItem
{
    /// <summary>
    /// Contains the ID information of the YouTube video (e.g., video ID).
    /// </summary>
    [JsonPropertyName("id")]
    public YouTubeVideoId Id { get; set; }

    /// <summary>
    /// Contains metadata about the YouTube video, such as title and thumbnails.
    /// </summary>
    [JsonPropertyName("snippet")]
    public YouTubeSnippet Snippet { get; set; }
}
