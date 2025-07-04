namespace VaporWebAPI.Services;

using System.Text.Json;
using System.Text.RegularExpressions;

using VaporWebAPI.ApiResponseModels;

public class JsonCleaningService
{
    private string steamApiKey;
    private readonly string filePath = @"C:\Users\User\Desktop\api.steampowered.com.json";
    private readonly string outputPath = @"C:\Users\User\Desktop\cleaned_music.json";
    //private string steamAPI = $"https://api.steampowered.com/IStoreService/GetAppList/v1/?key={steamApiKey}&include_games=false&include_dlc=false&include_software=true&include_videos=false&include_hardware=false";

    private static readonly List<string> bannedWords = new()
    {
        "hentai", "nsfw", "sex", "porn", "erotic", "nude", "fetish", "futa", "18+", "uncensored",
        "strip", "boobs", "adult", "xxx", "lewd", "naked", "sugar daddy", "sugar mom", "waifu",
        "bishoujo", "ecchi", "otome", "tentacle", "bdsm", "bondage", "sexy", "yaoi", "yuri", "daddy", "mature", "test",
        "slutty", "hentai", "adult", "slut", "anime", "maid", "milk", "baby", "neko", "domination", "mother"
    };

    public JsonCleaningService(IConfiguration configuration)
    {
        steamApiKey = configuration["SteamAPI:APIKey"];
    }


    /// <summary>
    /// Cleans the raw Steam JSON file by filtering out apps with banned words, invalid names,
    /// or that don't match the keyword "music". Saves the result to an output file.
    /// </summary>
    /// <returns>A message indicating the result of the cleaning process.</returns>
    public string CleanJsonFile()
    {
        if (!File.Exists(filePath))
        {
            return "File not found.";
        }

        string jsonData = File.ReadAllText(filePath);
        AppListResponse? steamData;
        try
        {
            steamData = JsonSerializer.Deserialize<AppListResponse>(jsonData);
        }
        catch (JsonException ex)
        {
            return $"JSON Deserialization failed: {ex.Message}";
        }

        if (steamData == null || steamData.AppList == null || steamData.AppList.Apps == null)
        {
            return "Invalid or empty JSON.";
        }

        int initialAppCount = steamData.AppList.Apps.Count;

        List<AppEntry> cleanedApps = steamData.AppList.Apps
            .Where(app => !string.IsNullOrWhiteSpace(app.Name) 
                        && IsValidName(app.Name) 
                        && !ContainsBannedWords(app.Name) 
                        && app.Name.Contains("music", StringComparison.OrdinalIgnoreCase)) 
            .ToList();

        int removedAppsCount = initialAppCount - cleanedApps.Count;

        steamData.AppList.Apps = cleanedApps;

        string cleanedJson = JsonSerializer.Serialize(steamData, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(outputPath, cleanedJson);

        return $"Cleaned JSON saved to: {outputPath}. Removed {removedAppsCount} inappropriate apps. Remaining apps: {steamData.AppList.Apps.Count}.";
    }

    /// <summary>
    /// Checks if a given app name contains any banned or inappropriate words.
    /// </summary>
    /// <param name="name">The name of the app to check.</param>
    /// <returns>True if a banned word is found; otherwise, false.</returns>
    private bool ContainsBannedWords(string name) =>
        bannedWords.Any(word => name.Contains(word, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Validates whether the app name matches the allowed character pattern.
    /// </summary>
    /// <param name="name">The app name to validate.</param>
    /// <returns>True if the name is valid; otherwise, false.</returns>
    private bool IsValidName(string name) =>
        Regex.IsMatch(name, "^[a-zA-Z0-9 _.'\\-&!?:]+$");
}
