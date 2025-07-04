namespace VaporWebAPI.Services;

using Microsoft.EntityFrameworkCore;

using VaporWebAPI.Data;
using VaporWebAPI.DTOs;
using VaporWebAPI.Models;

public class AppService
{
    private readonly VaporDbContext context;
    private readonly YouTubeService youTubeService;

    public AppService(VaporDbContext context, YouTubeService youTubeService)
    {
        this.context = context;
        this.youTubeService = youTubeService;
    }

    /// <summary>
    /// Retrieves the top 5 most purchased apps of type "game", including their videos, images, and genres.
    /// </summary>
    public async Task<List<StoreAppDTO>> GetTop5AppsAsync()
    {
        var topApps = await context.Apps
            .Where(a => a.AppTypeId == 1)
            .OrderByDescending(a => a.PurchaseCount)
            .Take(5)
            .Include(a => a.AppVideos)
            .Include(a => a.AppImages)
            .Include(a => a.AppGenres)
            .ThenInclude(ag => ag.Genre)
            .ToListAsync();

        return (await Task.WhenAll(topApps.Select(MapToStoreDTO))).ToList();
    }

    /// <summary>
    /// Retrieves a paginated list of apps of type "game", ordered by purchase count.
    /// </summary>
    /// <param name="page">The page number.</param>
    /// <param name="limit">Number of apps per page.</param>
    public async Task<List<StoreAppDTO>> GetPaginatedAppsAsync(int page, int limit)
    {
        var apps = await context.Apps
            .Where(a => a.AppTypeId == 1)
            .OrderByDescending(a => a.PurchaseCount)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Include(a => a.AppVideos)
            .Include(a => a.AppImages)
            .Include(a => a.AppGenres)
            .ThenInclude(ag => ag.Genre)
            .ToListAsync();

        return (await Task.WhenAll(apps.Select(MapToStoreDTO))).ToList();
    }

    /// <summary>
    /// Retrieves detailed information about a specific app by its ID.
    /// </summary>
    /// <param name="id">The ID of the app.</param>
    /// <returns>A detailed DTO with full app info or null if not found.</returns>
    public async Task<AppDetailsDTO?> GetAppByIdAsync(int id)
    {
        var app = await context.Apps
            .Include(a => a.AppGenres)
            .ThenInclude(ag => ag.Genre)
            .Include(a => a.AppVideos)
            .Include(a => a.AppImages)
            .Include(a => a.AppDevelopers)
            .ThenInclude(ad => ad.Developer)
            .Include(a => a.AppPublishers)
            .ThenInclude(ap => ap.Publisher)
            .FirstOrDefaultAsync(a => a.AppId == id);

        if (app == null)
            return null;

        return MapToDetailsDTO(app);
    }

    /// <summary>
    /// Retrieves all related apps (e.g., DLCs) for a given base app ID.
    /// </summary>
    /// <param name="baseAppId">The base app ID.</param>
    /// <returns>List of related apps mapped to store DTOs.</returns>
    public async Task<List<StoreAppDTO>> GetRelatedAppsAsync(int baseAppId)
    {
        var relatedApps = await context.Apps
            .Where(a => a.BaseAppId == baseAppId)
            .Include(a => a.AppGenres)
                .ThenInclude(ag => ag.Genre)
            .Include(a => a.AppVideos)
            .Include(a => a.AppImages)
            .Include(a => a.AppDevelopers)
                .ThenInclude(ad => ad.Developer)
            .Include(a => a.AppPublishers)
                .ThenInclude(ap => ap.Publisher)
            .ToListAsync();

        return (await Task.WhenAll(relatedApps.Select(MapToStoreDTO))).ToList();
    }

    /// <summary>
    /// Searches apps based on query text, price filter, genres, and optional sorting.
    /// </summary>
    /// <param name="query">Search keyword.</param>
    /// <param name="isFree">Whether to filter by free/paid apps.</param>
    /// <param name="genres">List of genres to filter by.</param>
    /// <param name="priceSort">Sort by price ("asc" or "desc").</param>
    public async Task<List<StoreAppDTO>> SearchAppsAsync(string? query, bool? isFree, List<string>? genres, string? priceSort)
    {
        query = query?.ToLower();
        var loweredGenres = genres?.Select(g => g.ToLower()).ToList();

        var apps = context.Apps
            .Where(a =>
                a.AppTypeId == 1 &&
                (string.IsNullOrEmpty(query) || a.AppName.ToLower().Contains(query)) &&
                (!isFree.HasValue || (isFree.Value ? a.Price == 0 : a.Price > 0)) &&
                (
                    loweredGenres == null || loweredGenres.Count == 0 ||
                    loweredGenres.All(g => a.AppGenres.Any(ag => ag.Genre.GenreName.ToLower() == g))
                )
            );

        if (isFree.HasValue)
        {
            apps = apps.Where(a => isFree.Value ? a.Price == 0 : a.Price > 0);
        }

        if (!string.IsNullOrEmpty(priceSort))
        {
            if (priceSort.ToLower() == "asc")
            {
                if (!isFree.HasValue)
                {
                    apps = apps.Where(a => a.Price > 0);
                }
                apps = apps.OrderBy(a => a.Price);
            }
            else if (priceSort.ToLower() == "desc")
            {
                apps = apps.OrderByDescending(a => a.Price);
            }
        }

        var finalApps = await apps
            .Include(a => a.AppImages)
            .Include(a => a.AppGenres).ThenInclude(ag => ag.Genre)
            .Include(a => a.AppVideos)
            .Include(a => a.AppType)
            .ToListAsync();

        return (await Task.WhenAll(finalApps.Select(MapToStoreDTO))).ToList();
    }

    /// <summary>
    /// Retrieves all distinct genre names, sorted alphabetically.
    /// </summary>
    public async Task<List<string>> GetAllGenresAsync()
    {
        return await context.Genres
            .OrderBy(g => g.GenreName)
            .Select(g => g.GenreName)
            .ToListAsync();
    }

    /// <summary>
    /// Maps an App entity to a simplified store DTO with trailer, price, and genre info.
    /// </summary>
    /// <param name="app">The App entity to map.</param>
    public async Task<StoreAppDTO> MapToStoreDTO(App app)
    {
        string trailerUrl = app.AppVideos.FirstOrDefault()?.VideoUrl ?? "";
        //string appType = context.AppTypes.FirstOrDefault(a => a.AppTypeId == app.AppTypeId).TypeName;
        string appType = app.AppType?.TypeName ?? "Unknown";
        string appName = app.AppName;

        //if (string.IsNullOrEmpty(trailerUrl))
        //{
        //    // Fetch from YouTube only if missing
        //    trailerUrl = await youTubeService.GetYouTubeVideoAsync(appName, appType) ?? "";
        //}

        return new StoreAppDTO
        {
            AppId = app.AppId,
            Name = app.AppName,
            HeaderImage = app.AppImages.FirstOrDefault(img => img.ImageType == "header")?.ImageUrl
                          ?? "/assets/appHeader.png",
            Price = app.Price.HasValue && app.Price != 0 ? $"{app.Price.Value}€" : "0",
            Trailer = trailerUrl,
            Genres = app.AppGenres.Select(ag => ag.Genre.GenreName).ToList(),
            ReleaseDate = app.ReleaseDate ?? "Unknown",
            ShortDescription = app.Description ?? "No description available."
        };
    }

    /// <summary>
    /// Maps an App entity to a detailed DTO including media, genres, developers, and publishers.
    /// </summary>
    /// <param name="app">The App entity.</param>
    public static AppDetailsDTO MapToDetailsDTO(App app)
    {
        return new AppDetailsDTO
        {
            AppId = app.AppId,
            BaseAppId = app.BaseAppId,
            Name = app.AppName,
            HeaderImage = app.AppImages.FirstOrDefault(img => img.ImageType == "header")?.ImageUrl
                          ?? "D:/repos/Vapor/client/src/assets/appHeader.png",
            Price = app.Price.HasValue ? $"{app.Price.Value}€" : "Free",
            Description = app.Description ?? "No description available.",
            ReleaseDate = app.ReleaseDate ?? "Unknown",

            Genres = app.AppGenres.Select(ag => ag.Genre.GenreName).ToList(),

            Developer = app.AppDevelopers.Select(ad => ad.Developer.DeveloperName).ToList(),
            Publisher = app.AppPublishers.Select(ap => ap.Publisher.PublisherName).ToList(),

            Screenshots = app.AppImages
                .Where(img => img.ImageType == "screenshot")
                .ToDictionary(img => img.ImageUrl, img => img.ThumbnailUrl ?? img.ImageUrl),

            Videos = app.AppVideos
                .ToDictionary(v => v.VideoUrl, v => v.ThumbnailUrl ?? v.VideoUrl)
        };
    }

    /// <summary>
    /// Retrieves all reviews for a specific app, ordered by newest first.
    /// </summary>
    /// <param name="appId">The ID of the app.</param>
    public async Task<List<AppReviewDTO>> GetReviewsForAppAsync(int appId)
    {
        return await context.AppReviews
            .Where(r => r.AppId == appId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new AppReviewDTO
            {
                ReviewId = r.ReviewId,
                AppId = r.AppId,
                UserDisplayName = r.UserDisplayName,
                Username = r.User != null ? r.User.Username : null,
                ProfilePictureUrl = r.User != null ? r.User.ProfilePicture : "/assets/defaultVaporProfilePic.jpg",
                IsRecommended = r.IsRecommended,
                ReviewText = r.ReviewText,
                CreatedAt = r.CreatedAt,
                IsEdited = r.IsEdited
            })
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new review for an app by a specific user if they haven't reviewed it already.
    /// </summary>
    /// <param name="userId">The ID of the user writing the review.</param>
    /// <param name="request">The review request data.</param>
    public async Task<string> AddReviewAsync(int userId, CreateReviewRequest request)
    {
        var user = await context.Users.FindAsync(userId);
        if (user == null) return "User not found";

        bool ownsApp = await context.AppLibraries
            .AnyAsync(al => al.UserId == userId && al.AppId == request.AppId);

        if (request.IsRecommended && !ownsApp)
            return "You must own the app to recommend it.";

        bool alreadyReviewed = await context.AppReviews
            .AnyAsync(r => r.AppId == request.AppId && r.UserId == userId);

        if (alreadyReviewed)
            return "You have already reviewed this app.";

        var review = new AppReview
        {
            AppId = request.AppId,
            UserId = userId,
            UserDisplayName = user.DisplayName,
            IsRecommended = request.IsRecommended,
            ReviewText = request.ReviewText,
            CreatedAt = DateTime.UtcNow,
            LastModification21180128 = DateTime.UtcNow
        };

        context.AppReviews.Add(review);
        await context.SaveChangesAsync();

        return "Success";
    }

    /// <summary>
    /// Checks whether a user has already reviewed a specific app.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="appId">The ID of the app.</param>
    public async Task<bool> HasUserReviewedAppAsync(int userId, int appId)
    {
        return await context.AppReviews
            .AnyAsync(r => r.AppId == appId && r.UserId == userId);
    }

    /// <summary>
    /// Checks whether a user owns a specific app.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="appId">The ID of the app.</param>
    public async Task<bool> UserOwnsAppAsync(int userId, int appId)
    {
        return await context.AppLibraries
            .AnyAsync(al => al.UserId == userId && al.AppId == appId);
    }

    /// <summary>
    /// Updates an existing review if the user is authorized and changes were made.
    /// </summary>
    /// <param name="userId">The ID of the user editing the review.</param>
    /// <param name="reviewId">The ID of the review to edit.</param>
    /// <param name="request">The updated review data.</param>
    public async Task<string> UpdateReviewAsync(int userId, int reviewId, CreateReviewRequest request)
    {
        var review = await context.AppReviews.FindAsync(reviewId);

        if (review == null)
            return "Review not found.";

        if (review.UserId != userId)
            return "You are not authorized to edit this review.";

        bool isChanged = false;

        if (review.ReviewText != request.ReviewText)
        {
            review.ReviewText = request.ReviewText;
            isChanged = true;
        }

        if (review.IsRecommended != request.IsRecommended)
        {
            review.IsRecommended = request.IsRecommended;
            isChanged = true;
        }

        if (isChanged)
        {
            review.IsEdited = true;
            await context.SaveChangesAsync();
            return "Updated";
        }

        return "No changes detected.";
    }
}
