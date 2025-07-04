namespace VaporWebAPI.Services;

using Microsoft.EntityFrameworkCore;

using VaporWebAPI.DTOs;
using VaporWebAPI.Data;
using VaporWebAPI.Models;

public class CartService
{
    private readonly VaporDbContext _context;

    public CartService(VaporDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves the current contents of a user's cart, including app details and header images.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <returns>List of cart items as DTOs.</returns>
    public async Task<List<CartItemDTO>> GetUserCartAsync(int userId)
    {
        return await _context.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.App)
                .ThenInclude(app => app.AppImages)
            .Select(c => new CartItemDTO
            {
                AppId = c.App.AppId,
                Name = c.App.AppName,
                Price = c.App.Price == 0 ? "Free" : $"{c.App.Price:0.00}",
                HeaderImage = c.App.AppImages
                    .Where(img => img.ImageType.ToLower() == "header")
                    .Select(img => img.ImageUrl)
                    .FirstOrDefault(),
                BaseAppId = c.App.BaseAppId
            })
            .ToListAsync();
    }

    /// <summary>
    /// Adds an app to the user's cart if it's not already present and validates base game ownership for related content.
    /// </summary>
    /// <param name="userId">The ID of the user adding the app.</param>
    /// <param name="appId">The ID of the app to add.</param>
    /// <exception cref="Exception">Thrown if the app or required base game is missing.</exception>
    public async Task AddToCartAsync(int userId, int appId)
    {
        var existing = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.AppId == appId);

        if (existing != null)
            return;
        
        var app = await _context.Apps.FindAsync(appId);
        if (app == null) throw new Exception("App not found");

        if (app.BaseAppId != null)
        {
            bool ownsBase = await _context.AppLibraries
                .AnyAsync(al => al.UserId == userId && al.AppId == app.BaseAppId);

            bool baseInCart = await _context.CartItems
                .AnyAsync(ci => ci.UserId == userId && ci.AppId == app.BaseAppId);

            if (!ownsBase && !baseInCart)
            {
                var baseApp = await _context.Apps.FindAsync(app.BaseAppId);
                throw new Exception($"You must own or have added to the cart the base game \"{baseApp?.AppName}\" before purchasing this content.");
            }
        }

        _context.CartItems.Add(new CartItem
        {
            UserId = userId,
            AppId = appId,
            Quantity = 1,
            LastModification21180128 = DateTime.Now
        });

        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Removes a specific app from the user's cart if it exists.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    /// <param name="appId">The ID of the app to remove.</param>
    public async Task RemoveFromCartAsync(int userId, int appId)
    {
        var item = await _context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.AppId == appId);

        if (item != null)
        {
            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Clears all items from the specified user's cart.
    /// </summary>
    /// <param name="userId">The ID of the user.</param>
    public async Task ClearCartAsync(int userId)
    {
        var items = _context.CartItems.Where(c => c.UserId == userId);
        _context.CartItems.RemoveRange(items);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Merges a guest cart into the logged-in user's cart by adding any apps
    /// that the user does not already own and that are not already in their cart.
    /// </summary>
    /// <param name="userId">The ID of the logged-in user.</param>
    /// <param name="guestAppIds">A list of app IDs from the guest session.</param>
    /// <remarks>
    /// This method avoids adding duplicates or apps the user already owns,
    /// ensuring only valid new cart items are merged.
    /// </remarks>
    public async Task MergeGuestCartAsync(int userId, List<int> guestAppIds)
    {
        foreach (var appId in guestAppIds)
        {
            bool alreadyOwned = await _context.AppLibraries
                .AnyAsync(a => a.UserId == userId && a.AppId == appId);
            if (alreadyOwned)
                continue;

            bool alreadyInCart = await _context.CartItems
                .AnyAsync(c => c.UserId == userId && c.AppId == appId);
            if (alreadyInCart)
                continue;

            _context.CartItems.Add(new CartItem
            {
                UserId = userId,
                AppId = appId,
                Quantity = 1,
                LastModification21180128 = DateTime.Now
            });
        }

        await _context.SaveChangesAsync();
    }
}
