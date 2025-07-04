namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a basic app entry returned by the Steam App List API.
/// </summary>
public class AppEntry
{
    /// <summary>
    /// The unique identifier of the app.
    /// </summary>
    [JsonPropertyName("appid")]
    public int AppId { get; set; }

    /// <summary>
    /// The name of the app.
    /// </summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
