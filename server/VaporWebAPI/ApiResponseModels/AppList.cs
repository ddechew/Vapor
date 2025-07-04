namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the list of apps retrieved from the Steam App List API.
/// </summary>
public class AppList
{
    /// <summary>
    /// A collection of all app entries.
    /// </summary>
    [JsonPropertyName("apps")]
    public List<AppEntry> Apps { get; set; }
}
