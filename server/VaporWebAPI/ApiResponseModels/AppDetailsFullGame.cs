namespace VaporWebAPI.ApiResponseModels;

using System.Text.Json.Serialization;

/// <summary>
/// Represents the 'fullgame' field in the Steam API response, used to extract base app ID for non-game apps (e.g., DLCs).
/// </summary>
public class AppDetailsFullGame
{
    [JsonPropertyName("appid")]
    public string? AppIdString { get; set; }  

    [JsonIgnore]
    public int? BaseAppId
    {
        get
        {
            if (int.TryParse(AppIdString, out int parsedId))
                return parsedId;
            return null; 
        }
    }
}
