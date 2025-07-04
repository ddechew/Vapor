namespace VaporWebAPI.Services;

using Microsoft.EntityFrameworkCore;

using VaporWebAPI.Data;
using VaporWebAPI.DTOs;
using VaporWebAPI.Models;

public class WishlistService
{
    private readonly VaporDbContext context;

    public WishlistService(VaporDbContext context)
    {
        this.context = context;
    }

    /// <summary>
    /// Retrieves the wishlist for a given user, including app details and images, sorted by priority.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>List of wishlist items mapped to DTOs.</returns>
    public async Task<List<WishlistAppDTO>> GetWishlistAsync(int userId)
    {
        var entries = await context.Wishlists
            .Where(w => w.UserId == userId)
            .Include(w => w.App)
                .ThenInclude(a => a.AppImages)
            .OrderBy(w => w.Priority)
            .ToListAsync();

        return entries.Select(MapToWishlistDTO).ToList();
    }

    /// <summary>
    /// Adds a new app to the user's wishlist if not already present, assigning the next highest priority.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="appId">The ID of the app to add.</param>
    /// <param name="priority">Requested priority (currently not used).</param>
    /// <returns>True if the app was added; false if it was already in the wishlist.</returns>
    public async Task<bool> AddToWishlistAsync(int userId, int appId, int priority)
    {
        if (await context.Wishlists.AnyAsync(w => w.UserId == userId && w.AppId == appId))
            return false;

        var userPriorities = await context.Wishlists
            .Where(w => w.UserId == userId)
            .Select(w => w.Priority)
            .ToListAsync();

        int maxPriority = userPriorities.Count > 0 ? userPriorities.Max() : 0;

        context.Wishlists.Add(new Wishlist
        {
            UserId = userId,
            AppId = appId,
            Priority = maxPriority + 1,
        });

        await context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Removes an app from the user's wishlist and normalizes the remaining priorities.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="appId">The ID of the app to remove.</param>
    /// <returns>True if removed successfully; false if not found.</returns>
    public async Task<bool> RemoveFromWishlistAsync(int userId, int appId)
    {
        var entry = await context.Wishlists.FirstOrDefaultAsync(w => w.UserId == userId && w.AppId == appId);
        if (entry == null) return false;

        context.Wishlists.Remove(entry);
        await context.SaveChangesAsync();

        await NormalizeWishlistPrioritiesAsync(userId);
        return true;
    }

    /// <summary>
    /// Updates the priority of a wishlist item, reordering the list accordingly.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="appId">The ID of the app to reprioritize.</param>
    /// <param name="newPriority">The new priority position (1-based).</param>
    /// <returns>True if successful; false if invalid app or priority.</returns>
    public async Task<bool> UpdatePriorityAsync(int userId, int appId, int newPriority)
    {
        var wishlist = await context.Wishlists
            .Where(w => w.UserId == userId)
            .OrderBy(w => w.Priority)
            .ToListAsync();

        var item = wishlist.FirstOrDefault(w => w.AppId == appId);
        if (item == null) return false;

        if (newPriority < 1 || newPriority > wishlist.Count)
            return false;

        wishlist.Remove(item);

        wishlist.Insert(newPriority - 1, item);

        for (int i = 0; i < wishlist.Count; i++)
        {
            wishlist[i].Priority = i + 1;
        }

        await context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Normalizes the wishlist priorities for a user to ensure sequential ordering starting from 1.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    public async Task NormalizeWishlistPrioritiesAsync(int userId)
    {
        var wishlist = await context.Wishlists
            .Where(w => w.UserId == userId)
            .OrderBy(w => w.Priority)
            .ToListAsync();

        for (int i = 0; i < wishlist.Count; i++)
        {
            wishlist[i].Priority = i + 1;
        }

        await context.SaveChangesAsync();
    }

    /// <summary>
    /// Maps a Wishlist entity to a WishlistAppDTO containing display info for the frontend.
    /// </summary>
    /// <param name="wishlist">The wishlist entry to map.</param>
    /// <returns>The corresponding DTO with app name, image, price, and priority.</returns>
    public static WishlistAppDTO MapToWishlistDTO(Wishlist wishlist)
    {
        return new WishlistAppDTO
        {
            AppId = wishlist.App.AppId,
            Name = wishlist.App.AppName,
            HeaderImage = wishlist.App.AppImages.FirstOrDefault(img => img.ImageType == "header")?.ImageUrl
                          ?? "/assets/appHeader.png",
            Price = wishlist.App.Price.HasValue && wishlist.App.Price != 0
                      ? $"{wishlist.App.Price.Value}€"
                      : "Free",
            Priority = wishlist.Priority
        };
    }
}
