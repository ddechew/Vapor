namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the outer wrapper of the Steam app details API response,
/// indicating success status and containing the main application data.
/// </summary>
public class AppDetailsWrapper
{
    /// <summary>
    /// Indicates whether the API call was successful.
    /// </summary>
    [JsonPropertyName("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Contains detailed information about the app, including name, price, description, media, etc.
    /// </summary>
    [JsonPropertyName("data")]
    public AppDetailsResponse Data { get; set; }
}
