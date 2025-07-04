namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the response from a YouTube API search request.
/// </summary>
public class YouTubeSearchResponse
{
    /// <summary>
    /// A list of video items returned by the YouTube search.
    /// </summary>
    [JsonPropertyName("items")]
    public List<YouTubeItem> Items { get; set; }
}
