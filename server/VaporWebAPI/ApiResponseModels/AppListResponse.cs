namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the top-level response object from the Steam App List API.
/// </summary>
public class AppListResponse
{
    /// <summary>
    /// Contains the list of applications returned in the API response.
    /// </summary>
    //[JsonPropertyName("response")]
    [JsonPropertyName("applist")]
    public AppList AppList { get; set; } 
}
