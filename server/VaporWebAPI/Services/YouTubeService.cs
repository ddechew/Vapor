namespace VaporWebAPI.Services;

using Microsoft.EntityFrameworkCore;

using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using VaporWebAPI.ApiResponseModels;
using VaporWebAPI.Data;
using VaporWebAPI.Models;

public class YouTubeService
{
    private readonly HttpClient httpClient;
    private readonly string apiKey;
    private readonly VaporDbContext context;

    public YouTubeService(HttpClient httpClient, IConfiguration configuration, VaporDbContext context)
    {
        this.httpClient = httpClient;
        apiKey = configuration["YouTubeAPIKey:API_KEY"];
        this.context = context;
    }

    /// <summary>
    /// Searches YouTube for the most relevant game trailer based on the provided game name.
    /// </summary>
    /// <param name="gameName">The name of the game to search for.</param>
    /// <returns>
    /// A tuple containing the video URL and thumbnail URL, or null values if no result is found.
    /// </returns>
    public async Task<(string? videoUrl, string? thumbnailUrl)> GetYouTubeTrailerAsync(string gameName)
    {
        try
        {
            string query = Uri.EscapeDataString($"{gameName} game trailer");
            string url = $"https://www.googleapis.com/youtube/v3/search?part=snippet&q={query}&type=video&videoCategoryId=20&order=relevance&maxResults=1&key={apiKey}";

            var response = await httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"YouTube API Error: {response.StatusCode}");
                return (null, null);
            }

            string json = await response.Content.ReadAsStringAsync();
            var data = JsonSerializer.Deserialize<YouTubeSearchResponse>(json);

            if (data?.Items?.Count > 0)
            {
                string videoId = data.Items[0].Id.VideoId;
                string videoUrl = $"https://www.youtube.com/watch?v={videoId}";
                string thumbnailUrl = data.Items[0].Snippet.Thumbnails.Default.Url;

                return (videoUrl, thumbnailUrl);
            }

            return (null, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"YouTube API Exception: {ex.Message}");
            return (null, null);
        }
    }

    /// <summary>
    /// Fetches YouTube trailers for all apps of type "game" that currently have no associated video,
    /// and adds the video information to the database.
    /// </summary>
    /// <returns>The number of apps successfully updated with YouTube trailers.</returns>
    public async Task<int> UpdateAppsWithYouTubeTrailersAsync()
    {
        var apps = await context.Apps
            .Where(a => a.AppTypeId == 1 && !context.AppVideos.Any(v => v.AppId == a.AppId))
            .ToListAsync();

        int updatedCount = 0;

        foreach (var app in apps)
        {
            var (videoUrl, thumbnailUrl) = await GetYouTubeTrailerAsync(app.AppName);
            if (!string.IsNullOrEmpty(videoUrl))
            {
                var videoId = new Uri(videoUrl).Query.Split("v=")[1];
                var embedUrl = $"https://www.youtube.com/embed/{videoId}";

                var appVideo = new AppVideo
                {
                    AppId = app.AppId,
                    VideoUrl = embedUrl,
                    ThumbnailUrl = thumbnailUrl,
                    LastModification21180128 = DateTime.Now
                };

                context.AppVideos.Add(appVideo);
                updatedCount++;
            }
        }

        await context.SaveChangesAsync();
        return updatedCount;
    }
}