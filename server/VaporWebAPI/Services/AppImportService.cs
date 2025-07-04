namespace VaporWebAPI.Services;

using System.Text.Json;

using Microsoft.EntityFrameworkCore;

using VaporWebAPI.Models;
using VaporWebAPI.ApiResponseModels;
using VaporWebAPI.Utils;
using VaporWebAPI.Data;

public class AppImportService
{
    //private readonly string cleanedFilePath = @"C:\Users\User\Desktop\cleaned_games.json";
    private readonly string cleanedFilePath = @"C:\Users\User\Desktop\cleaned_custom.json";
    private readonly VaporDbContext context;
    private readonly HttpClient httpClient;
    private readonly YouTubeService youTubeService;

    public AppImportService(VaporDbContext context, HttpClient httpClient, YouTubeService youTubeService)
    {
        this.context = context;
        this.httpClient = httpClient;
        this.youTubeService = youTubeService;
    }

    /// <summary>
    /// Imports applications from a cleaned JSON file, fetches their details from the Steam API,
    /// and inserts them into the database with related metadata (genres, developers, etc.).
    /// </summary>
    public async Task<string> ImportAppsAsync()
    {
        if (!File.Exists(cleanedFilePath))
        {
            return "Cleaned file not found! Run JSON cleaning first.";
        }

        string jsonData = await File.ReadAllTextAsync(cleanedFilePath);
        var appListResponse = JsonSerializer.Deserialize<AppListResponse>(jsonData);

        if (appListResponse?.AppList?.Apps == null || appListResponse.AppList.Apps.Count == 0)
        {
            return "No apps left to import.";
        }

        var appsToProcess = appListResponse.AppList.Apps.ToList();

        foreach (var app in appsToProcess)
        {
            if (await context.Apps.AnyAsync(a => a.AppId == app.AppId))
            {
                RemoveAppFromJson(app, appListResponse);
                continue;
            }

            var appDetails = await FetchAppDetailsAsync(app.AppId);
            if (appDetails == null || !IsValidAppDetails(appDetails))
            {
                RemoveAppFromJson(app, appListResponse);
                continue;
            }

            int? baseAppId = null;

            if (appDetails.AppType != "game" && appDetails.FullGame != null && appDetails.FullGame.BaseAppId != null)
            {
                int? potentialBaseAppId = appDetails.FullGame.BaseAppId;

                if (potentialBaseAppId.HasValue)
                {
                    int baseId = potentialBaseAppId.Value;
                    bool baseExistsInDb = await context.Apps.AnyAsync(a => a.AppId == potentialBaseAppId);

                    if (!baseExistsInDb)
                    {
                        var baseGameDetails = await FetchAppDetailsAsync(baseId);

                        if (baseGameDetails == null || baseGameDetails.AppType != "game")
                        {
                            RemoveAppFromJson(app, appListResponse);
                            continue;
                        }

                        await InsertFullAppAsync(new AppEntry { AppId = baseId }, baseGameDetails);
                    }

                    baseAppId = baseId;
                }
            }

            await InsertFullAppAsync(app, appDetails, baseAppId);
            RemoveAppFromJson(app, appListResponse);

             await Task.Delay(1500);
        }

        return "Apps import process completed.";
    }

    /// <summary>
    /// Inserts a full application record into the database, including its details, images, and videos.
    /// </summary>
    /// <param name="app">The basic app entry.</param>
    /// <param name="appDetails">The detailed app information.</param>
    /// <param name="baseAppId">Optional base app ID (for DLCs and similar content).</param>
    private async Task InsertFullAppAsync(AppEntry app, AppDetailsResponse appDetails, int? baseAppId = null)
    {
        int appTypeId = await GetOrCreateAppTypeId(appDetails.AppType, baseAppId);

        decimal priceInEuro = appDetails.IsFree ? 0m : GetPriceInEuro(appDetails.PriceOverview);

        var newApp = new App
        {
            AppId = app.AppId,
            AppName = appDetails.AppName,
            BaseAppId = baseAppId,
            Description = appDetails.Description,
            Price = priceInEuro,
            AppTypeId = appTypeId,
            ReleaseDate = appDetails.ReleaseDate?.Date ?? "Unknown",
            LastModification21180128 = DateTime.Now
        };

        await context.Apps.AddAsync(newApp);
        await context.SaveChangesAsync();

        await InsertAdditionalAppDetailsAsync(app.AppId, appDetails);
        await InsertAppImagesAsync(app.AppId, appDetails);
        await InsertAppVideosAsync(app.AppId, appDetails);
    }

    /// <summary>
    /// Removes an app from the cleaned JSON file after processing or skipping.
    /// </summary>
    /// <param name="app">The app to remove.</param>
    /// <param name="appListResponse">The deserialized JSON object containing the app list.</param>
    private async void RemoveAppFromJson(AppEntry app, AppListResponse appListResponse)
    {
        appListResponse.AppList.Apps.Remove(app);
        string updatedJson = JsonSerializer.Serialize(appListResponse, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(cleanedFilePath, updatedJson);
    }

    /// <summary>
    /// Fetches detailed app information from the Steam Store API for a given App ID.
    /// </summary>
    /// <param name="appId">The ID of the app to fetch.</param>
    /// <returns>The detailed app response or null if not found/invalid.</returns>
    private async Task<AppDetailsResponse?> FetchAppDetailsAsync(int appId)
    {
        string url = $"https://store.steampowered.com/api/appdetails?appids={appId}";
        var response = await httpClient.GetStringAsync(url);

        if (string.IsNullOrEmpty(response))
        {
            return null;
        }

        var steamData = JsonSerializer.Deserialize<Dictionary<string, AppDetailsWrapper>>(response);

        if (steamData == null || !steamData.ContainsKey(appId.ToString()) || !steamData[appId.ToString()].Success)
        {
            return null;
        }

        return steamData[appId.ToString()].Data; 
    }

    /// <summary>
    /// Validates whether the fetched app details are usable and free from banned content.
    /// </summary>
    /// <param name="appDetails">The app details to validate.</param>
    /// <returns>True if valid; otherwise, false.</returns>
    private bool IsValidAppDetails(AppDetailsResponse appDetails)
    {
        return !string.IsNullOrWhiteSpace(appDetails.AppName) &&
               !ContainsBannedWords(appDetails.AppName) &&
               !ContainsBannedWords(appDetails.Description);
    }

    /// <summary>
    /// Inserts related data such as developers, publishers, and genres for an application.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    /// <param name="appDetails">The app details containing related data.</param>
    private async Task InsertAdditionalAppDetailsAsync(int appId, AppDetailsResponse appDetails)
    {
        if (appDetails.Developers != null)
        {
            foreach (var devName in appDetails.Developers)
            {
                var developer = await context.Developers.FirstOrDefaultAsync(d => d.DeveloperName == devName);
                if (developer == null)
                {
                    developer = new Developer { DeveloperName = devName, LastModification21180128 = DateTime.Now };
                    await context.Developers.AddAsync(developer);
                    await context.SaveChangesAsync();
                }

                await context.AppDevelopers.AddAsync(new AppDeveloper
                {
                    AppId = appId,
                    DeveloperId = developer.DeveloperId,
                    LastModification21180128 = DateTime.Now
                });
            }
        }

        if (appDetails.Publishers != null)
        {
            foreach (var pubName in appDetails.Publishers)
            {
                var publisher = await context.Publishers.FirstOrDefaultAsync(p => p.PublisherName == pubName);
                if (publisher == null)
                {
                    publisher = new Publisher { PublisherName = pubName, LastModification21180128 = DateTime.Now };
                    await context.Publishers.AddAsync(publisher);
                    await context.SaveChangesAsync();
                }

                await context.AppPublishers.AddAsync(new AppPublisher
                {
                    AppId = appId,
                    PublisherId = publisher.PublisherId,
                    LastModification21180128 = DateTime.Now
                });
            }
        }

        if (appDetails.Genres != null)
        {
            foreach (var genreDetail in appDetails.Genres)
            {
                if (!int.TryParse(genreDetail.Id, out int externalGenreId))
                {
                    continue;
                }

                var existingGenre = await context.Genres
                        .FirstOrDefaultAsync(g => g.ExternalGenreId == externalGenreId);

                if (existingGenre == null)
                {
                    existingGenre = new Genre
                    {
                        GenreName = genreDetail.Description,
                        ExternalGenreId = externalGenreId, 
                        LastModification21180128 = DateTime.Now
                    };

                    await context.Genres.AddAsync(existingGenre);
                    await context.SaveChangesAsync();
                }

                await context.AppGenres.AddAsync(new AppGenre
                {
                    AppId = appId,
                    GenreId = existingGenre.GenreId,
                    LastModification21180128 = DateTime.Now
                });

                await context.SaveChangesAsync();
            }
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Inserts all image assets (header and screenshots) for an app into the database.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    /// <param name="appDetails">The app details containing image URLs.</param>
    private async Task InsertAppImagesAsync(int appId, AppDetailsResponse appDetails)
    {
        string defaultImagePath = @"D:\repos\Vapor\client\src\assets\appHeader.png";

        var headerImage = new AppImage
        {
            AppId = appId,
            ImageUrl = !string.IsNullOrEmpty(appDetails.HeaderImage) ? appDetails.HeaderImage : defaultImagePath,
            ImageType = "header",
            LastModification21180128 = DateTime.Now
        };
        await context.AppImages.AddAsync(headerImage);

        if (appDetails.Screenshots != null)
        {
            foreach (var screenshot in appDetails.Screenshots)
            {
                var appImage = new AppImage
                {
                    AppId = appId,
                    ImageUrl = screenshot.FullImage,
                    ThumbnailUrl = screenshot.Thumbnail,
                    ImageType = "screenshot",
                    LastModification21180128 = DateTime.Now
                };
                await context.AppImages.AddAsync(appImage);
            }
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Inserts all video assets for an app, using Steam or YouTube if missing.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    /// <param name="appDetails">The app details containing video information.</param>
    private async Task InsertAppVideosAsync(int appId, AppDetailsResponse appDetails)
    {
        if (appDetails.Videos == null && appDetails.AppType.ToLower() == "game")
        {
            var (videoUrl, thumbnailUrl) = await youTubeService.GetYouTubeTrailerAsync(appDetails.AppName);
            AppVideo appVideo = new()
            {
                AppId = appId,
                VideoUrl = videoUrl,
                ThumbnailUrl = thumbnailUrl,
                LastModification21180128 = DateTime.Now
            };
            await context.AppVideos.AddAsync(appVideo);
            await context.SaveChangesAsync();
            return;
        }

        if (appDetails.Videos == null)
        {
            return;
        }

        foreach (var video in appDetails?.Videos)
        {
            var videoUrl = video.Mp4?.FirstOrDefault().Value ?? "";
            if (!string.IsNullOrEmpty(videoUrl))
            {
                var appVideo = new AppVideo
                {
                    AppId = appId,
                    VideoUrl = videoUrl,
                    ThumbnailUrl = video.Thumbnail,
                    LastModification21180128 = DateTime.Now
                };
                await context.AppVideos.AddAsync(appVideo);
            }
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Checks if a string contains inappropriate or banned words.
    /// </summary>
    /// <param name="name">The string to scan.</param>
    /// <returns>True if banned words are found; otherwise, false.</returns>
    private bool ContainsBannedWords(string name) =>
        new List<string> { "hentai", "nsfw", "sex", "porn", "erotic", "nude", "fetish", "futa", "18+" }
        .Any(word => name.Contains(word, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Retrieves an existing AppType ID or creates a new one if it does not exist.
    /// </summary>
    /// <param name="appType">The app type string from Steam.</param>
    /// <param name="baseAppId">Optional base app ID to determine fallback logic.</param>
    /// <returns>The ID of the app type.</returns>
    private async Task<int> GetOrCreateAppTypeId(string appType, int? baseAppId)
    {
        var existingAppType = await context.AppTypes.FirstOrDefaultAsync(at => at.TypeName.ToLower() == appType);

        if (existingAppType != null)
        {
            return existingAppType.AppTypeId;
        }

        var newAppType = new AppType
        {
            TypeName = appType,
            LastModification21180128 = DateTime.Now
        };

        await context.AppTypes.AddAsync(newAppType);
        await context.SaveChangesAsync();

        return newAppType.AppTypeId;
    }

    /// <summary>
    /// Converts a price from local currency to EUR using predefined exchange rates.
    /// </summary>
    /// <param name="priceOverview">The price overview object.</param>
    /// <returns>The price in euros.</returns>
    private decimal GetPriceInEuro(AppDetailsPriceOverview priceOverview)
    {
        if (priceOverview == null)
        {
            return 0m;
        }

        decimal amount = priceOverview.FinalPrice / 100m;
        string currencyCode = priceOverview.Currency.ToUpper();

        if (CurrencyExchangeRates.ExchangeRatesToEuro.TryGetValue(currencyCode, out var exchangeRate))
        {
            return Math.Round(amount * exchangeRate, 2);
        }

        return 0m;
    }
}
