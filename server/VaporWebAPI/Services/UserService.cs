namespace VaporWebAPI.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System.IO;
using System.ComponentModel.DataAnnotations;

using VaporWebAPI.Data;
using VaporWebAPI.DTOs;
using VaporWebAPI.Models;
using VaporWebAPI.Services.Interfaces;
using VaporWebAPI.Utils;

public class UserService
{
    private readonly VaporDbContext _context;
    private readonly IEmailSender emailSender;

    public UserService(VaporDbContext context, IEmailSender emailSender)
    {
        _context = context;
        this.emailSender = emailSender;
    }

    /// <summary>
    /// Retrieves user details by user ID.
    /// </summary>
    public async Task<User?> GetUserDetailsAsync(int userId)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    /// <summary>
    /// Retrieves a user by their username.
    /// </summary>
    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username);
    }

    /// <summary>
    /// Updates the user's profile display name and profile picture, and updates related post/comment display names.
    /// </summary>
    public async Task<string> UpdateUserProfileAsync(string username, UpdateProfileRequest request)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return "User not found";

        if (!string.IsNullOrWhiteSpace(request.DisplayName))
        {
            string displayNameFeedback = InputValidation.ValidateDisplayName(request.DisplayName);
            if (displayNameFeedback != "Valid")
            {
                return displayNameFeedback;
            }

            user.DisplayName = request.DisplayName;
        }

        user.ProfilePicture = request.ProfilePicture ?? user.ProfilePicture;

        await _context.PostComments
        .Where(c => c.UserId == user.UserId && c.UserDisplayName != "[deleted]")
        .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.UserDisplayName, user.DisplayName));

        await _context.Posts
        .Where(c => c.UserId == user.UserId && c.UserDisplayName != "[deleted]")
        .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.UserDisplayName, user.DisplayName));

        await _context.SaveChangesAsync();
        return "Success";
    }

    /// <summary>
    /// Uploads and sets a new profile picture for the user, replacing the old one if it exists.
    /// </summary>
    public async Task<string?> UploadProfilePictureAsync(string username, IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return null;

        if (!string.IsNullOrEmpty(user.ProfilePicture) &&
            user.ProfilePicture.StartsWith("/assets/") &&
            !user.ProfilePicture.Contains("defaultVapor"))
        {
            var oldFileName = Path.GetFileName(user.ProfilePicture);
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "client", "public", "assets", oldFileName);

            if (File.Exists(oldFilePath))
            {
                try
                {
                    File.Delete(oldFilePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("⚠ Failed to delete old profile picture: " + ex.Message);
                }
            }
        }

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var relativePath = Path.Combine("assets", fileName);
        var clientPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "client", "public", relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(clientPath)!);

        using (var stream = new FileStream(clientPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        user.ProfilePicture = $"/assets/{fileName}";
        await _context.SaveChangesAsync();

        return user.ProfilePicture;
    }

    /// <summary>
    /// Initiates an email change request by sending a confirmation email to the user's current address.
    /// </summary>
    public async Task<string> InitiateEmailChangeAsync(string username, string newEmail)
    {
        if (string.IsNullOrWhiteSpace(newEmail))
            return "Email is required.";

        if (!new EmailAddressAttribute().IsValid(newEmail))
            return "Invalid email format.";

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
            return "User not found.";

        if (user.Email.Trim().ToLower() == newEmail.Trim().ToLower())
            return "Email change is not allowed."; // ❌ vague to avoid disclosure

        bool emailExists = await _context.Users
            .AnyAsync(u => u.Email.ToLower() == newEmail.ToLower() && u.UserId != user.UserId);

        if (emailExists)
            return "Email change is not allowed.";

        var existingToken = await _context.EmailChangeTokens
            .FirstOrDefaultAsync(t => t.UserId == user.UserId);

        if (existingToken != null)
        {
            if (existingToken.ExpiresAt < DateTime.UtcNow)
            {
                _context.EmailChangeTokens.Remove(existingToken);
            }
            else
            {
                return "You already have a pending email change request. Please confirm or wait for it to expire.";
            }
        }


        string token = Guid.NewGuid().ToString();

        var emailToken = new EmailChangeToken
        {
            UserId = user.UserId,
            NewEmail = newEmail,
            Token = token,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(10),
        };

        _context.EmailChangeTokens.Add(emailToken);
        await _context.SaveChangesAsync();

        var model = new EmailChangeRequestModel
        {
            Username = user.Username,
            NewEmail = newEmail,
            Link = $"https://localhost:3000/confirm-email-change?token={token}"
        };

        var html = await EmailTemplateRenderer.RenderAsync("EmailChangeRequest", "EmailChangeRequest.cshtml", model);
        await emailSender.SendAsync(user.Email, "Confirm your email change", html);

        return "Confirmation email sent.";
    }

    /// <summary>
    /// Confirms and applies the email change using the provided token.
    /// </summary>
    public async Task<string> ChangeEmailAsync(string token)
    {
        var record = await _context.EmailChangeTokens
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.Token == token);

        if (record == null)
            return "Invalid or expired token.";

        if (record.ExpiresAt < DateTime.UtcNow)
        {
            _context.Remove(record);
            await _context.SaveChangesAsync();
            return "Invalid or expired token.";
        }

        record.User.Email = record.NewEmail;
        _context.EmailChangeTokens.Remove(record);

        await _context.SaveChangesAsync();

        var notifyModel = new EmailVerificationModel
        {
            Username = record.User.Username
        };

        var notifyHtml = await EmailTemplateRenderer.RenderAsync("EmailChanged", "EmailChanged.cshtml", notifyModel);
        await emailSender.SendAsync(record.NewEmail, "Your email has been changed", notifyHtml);

        return "Success";
    }

    /// <summary>
    /// Changes the user's password after validating the current one and the new password's strength.
    /// </summary>
    public async Task<string> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return "User not found.";

        var hasher = new PasswordHasher<string>();
        var result = hasher.VerifyHashedPassword(null, user.PasswordHash, currentPassword);

        if (result != PasswordVerificationResult.Success)
            return "Current password is incorrect.";

        string passwordFeedback = InputValidation.ValidatePassword(newPassword);
        if (passwordFeedback != "Valid")
            return passwordFeedback;

        user.PasswordHash = hasher.HashPassword(null, newPassword);
        await _context.SaveChangesAsync();

        var model = new EmailVerificationModel()
        {
            Username = user.Username
        };

        var emailHtml = await EmailTemplateRenderer.RenderAsync("PasswordChanged", "PasswordChanged.cshtml", model);

        await emailSender.SendAsync(user.Email, "Password has been changed!", emailHtml);

        return "Success";
    }

    /// <summary>
    /// Changes the username for a user after validating uniqueness and format.
    /// </summary>
    public async Task<string> ChangeUsernameAsync(int userId, string newUsername)
    {
        string validation = InputValidation.ValidateUsername(newUsername);
        if (validation != "Valid")
            return validation;

        bool exists = await _context.Users.AnyAsync(u => u.Username.ToLower() == newUsername.ToLower());
        if (exists)
            return "Username is already taken.";

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return "User not found.";

        string oldUsername = user.Username;
        user.Username = newUsername;


        await _context.SaveChangesAsync();

        var model = new EmailVerificationModel
        {
            Username = newUsername
        };

        var emailHtml = await EmailTemplateRenderer.RenderAsync("UsernameChanged", "UsernameChanged.cshtml", model);
        await emailSender.SendAsync(user.Email, "Username successfully changed!", emailHtml);

        return "Success";
    }

    /// <summary>
    /// Unlinks the user's Google account if they have set a password.
    /// </summary>
    public async Task<string> UnlinkGoogleAccountAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return "User not found";

        if (!user.IsGoogleAuthenticated || string.IsNullOrEmpty(user.GoogleId))
            return "Your account is not linked with Google.";

        if (string.IsNullOrWhiteSpace(user.PasswordHash) || user.PasswordHash == "GOOGLE_AUTH")
            return "Please set a password before unlinking your Google account.";

        user.IsGoogleAuthenticated = false;
        user.GoogleId = null;

        await _context.SaveChangesAsync();

        var model = new EmailVerificationModel
        {
            Username = user.Username
        };

        var html = await EmailTemplateRenderer.RenderAsync("GoogleUnlinked", "GoogleUnlinked.cshtml", model);
        await emailSender.SendAsync(user.Email, "Google Account Unlinked", html); ;

        return "Success";
    }

    /// <summary>
    /// Processes a purchase using the user's wallet, applies point-based discounts,
    /// updates ownership and sends an invoice email.
    /// </summary>
    public async Task<string> PurchaseWithWalletAsync(int userId, int pointsToUse)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return "User not found";

        if (pointsToUse > user.Points)
        {
            return "You cannot use more points than you have.";
        }

        var cartItems = await _context.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.App)
                .ThenInclude(a => a.AppImages)
            .ToListAsync();

        if (!cartItems.Any())
            return "Cart is empty";

        decimal totalPrice = 0;

        var appIdsInCart = cartItems.Select(c => c.AppId).ToHashSet();

        foreach (var item in cartItems)
        {
            if (item.App.BaseAppId != null)
            {
                bool ownsOrWillOwnBaseApp = await _context.AppLibraries
                    .AnyAsync(al => al.UserId == userId && al.AppId == item.App.BaseAppId)
                    || appIdsInCart.Contains(item.App.BaseAppId.Value);

                if (!ownsOrWillOwnBaseApp)
                    return $"You must own or be buying the base game before purchasing '{item.App.AppName}'";
            }

            totalPrice += item.App.Price ?? 0m;

        }

        int discountPercent = 0;
        int actualPointsUsed = 0;

        if (pointsToUse >= 500 && user.Points >= 500)
        {
            discountPercent = 50;
            actualPointsUsed = 500;
        }
        else if (pointsToUse >= 250 && user.Points >= 250)
        {
            discountPercent = 25;
            actualPointsUsed = 250;
        }
        else if (pointsToUse >= 100 && user.Points >= 100)
        {
            discountPercent = 10;
            actualPointsUsed = 100;
        }

        if (pointsToUse > 0 && actualPointsUsed == 0)
        {
            return "Invalid discount attempt.";
        }

        if (discountPercent > 0)
        {
            totalPrice = Math.Round(totalPrice * (1 - discountPercent / 100m), 2);
            user.Points -= actualPointsUsed;
        }

        var discountMultiplier = 1 - discountPercent / 100m;


        var roundedWallet = Math.Round(user.Wallet, 2);
        var finalPrice = Math.Round(totalPrice, 2);

        if (roundedWallet < finalPrice)
            return "Insufficient wallet balance";

        user.Wallet = roundedWallet - finalPrice;

        if (discountPercent == 0)
        {
            user.Points += (int)Math.Floor(totalPrice);
        }

        var currentWalletBalance = user.Wallet + finalPrice;

        foreach (var item in cartItems)
        {
            var alreadyOwned = await _context.AppLibraries
                .AnyAsync(al => al.UserId == userId && al.AppId == item.AppId);

            if (!alreadyOwned)
            {
                _context.AppLibraries.Add(new AppLibrary
                {
                    UserId = userId,
                    AppId = item.AppId,
                    LastModification21180128 = DateTime.Now
                });

                item.App.PurchaseCount += 1;

                var wishlistItem = await _context.Wishlists
                    .FirstOrDefaultAsync(w => w.UserId == userId && w.AppId == item.AppId);
                if (wishlistItem != null)
                {
                    _context.Wishlists.Remove(wishlistItem);
                }
            }

            var price = item.App.Price ?? 0;
            var discountedPrice = Math.Round(price * discountMultiplier, 2);
            currentWalletBalance -= discountedPrice;

            _context.PurchaseHistories.Add(new PurchaseHistory
            {
                UserId = userId,
                AppId = item.AppId,
                PurchaseDate = DateTime.Now,
                PriceAtPurchase = discountedPrice,
                PaymentMethod = "Wallet",
                WalletChange = -discountedPrice,
                WalletBalanceAfter = currentWalletBalance,
            });


        }

        _context.CartItems.RemoveRange(cartItems);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("🔥 DB Update Exception: " + ex.InnerException?.Message ?? ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("🔥 General Exception: " + ex.Message);
            throw;
        }

        var originalTotal = cartItems.Sum(item => item.App.Price ?? 0);
        var discountedTotal = Math.Round(originalTotal * discountMultiplier, 2);

        var invoiceModel = new PurchaseInvoiceModel
        {
            Username = user.Username,
            PurchaseDate = DateTime.Now,
            Items = cartItems.Select(item => new PurchaseInvoiceItem
            {
                Name = item.App.AppName,
                HeaderImage = item.App.AppImages
                    .Where(img => img.ImageType == "header")
                    .Select(img => img.ImageUrl)
                    .FirstOrDefault() ?? "/assets/default-header.jpg",
                PriceAtPurchase = Math.Round((item.App.Price ?? 0) * discountMultiplier, 2),
                OriginalPrice = item.App.Price
            }).ToList(),
            OriginalTotal = discountPercent > 0 ? Math.Round(originalTotal, 2) : null,
            DiscountPercent = discountPercent > 0 ? discountPercent : null
        };

        var invoiceHtml = await EmailTemplateRenderer.RenderAsync("PurchaseInvoice", "PurchaseInvoice.cshtml", invoiceModel);
        await emailSender.SendAsync(user.Email, "Your Vapor Invoice", invoiceHtml);

        return "Success";
    }

    /// <summary>
    /// Adds a free app to the user's library if it's not already owned and base game conditions are met.
    /// </summary>
    public async Task<string> AddFreeAppToLibraryAsync(int userId, int appId)
    {
        var app = await _context.Apps.FindAsync(appId);
        if (app == null) return "App not found.";
        if (app.Price > 0) return "App is not free.";

        var alreadyOwned = await _context.AppLibraries
            .AnyAsync(al => al.UserId == userId && al.AppId == appId);

        if (alreadyOwned) return "App already owned.";

        if (app.BaseAppId != null)
        {
            bool ownsBaseGame = await _context.AppLibraries
                .AnyAsync(al => al.UserId == userId && al.AppId == app.BaseAppId);

            if (!ownsBaseGame)
                return $"You must own the base game before adding '{app.AppName}' to your library.";
        }

        _context.AppLibraries.Add(new AppLibrary
        {
            UserId = userId,
            AppId = appId,
            LastModification21180128 = DateTime.Now
        });

        var wishlistItem = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.AppId == app.AppId);

        if (wishlistItem != null)
        {
            _context.Wishlists.Remove(wishlistItem);
        }

        _context.PurchaseHistories.Add(new PurchaseHistory
        {
            UserId = userId,
            AppId = appId,
            PurchaseDate = DateTime.Now,
            PriceAtPurchase = 0,
            PaymentMethod = "Free",
            WalletChange = null,
            WalletBalanceAfter = null
        });

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            Console.WriteLine("🔥 DB Update Exception: " + ex.InnerException?.Message ?? ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine("🔥 General Exception: " + ex.Message);
            throw;
        }

        return "Success";
    }

    /// <summary>
    /// Retrieves a list of all apps owned by the user, along with related content like DLCs or extras.
    /// </summary>
    public async Task<List<UserLibraryItemDTO>> GetUserLibraryAsync(int userId)
    {
        var ownedAppIds = await _context.AppLibraries
            .Where(al => al.UserId == userId)
            .Select(al => al.AppId)
            .ToListAsync();

        var ownedBaseApps = await _context.Apps
            .Where(app => ownedAppIds.Contains(app.AppId) && app.BaseAppId == null)
            .Include(a => a.AppImages)
            .ToListAsync();

        var result = new List<UserLibraryItemDTO>();

        foreach (var baseApp in ownedBaseApps)
        {
            var relatedApps = await _context.Apps
                .Where(a => a.BaseAppId == baseApp.AppId && a.AppTypeId != 4) 
                .Include(a => a.AppImages)
            .ToListAsync();

            var relatedDtos = relatedApps.Select(related => new RelatedAppDTO
            {
                AppId = related.AppId,
                AppName = related.AppName,
                Price = related.Price,
                AppTypeId = related.AppTypeId,
                IsOwned = ownedAppIds.Contains(related.AppId),
                HeaderImage = related.AppImages
                    .Where(x => x.AppId == related.AppId && x.ImageType == "header")
                    .Select(x => x.ImageUrl)
                    .FirstOrDefault()
            }).ToList();

            var appLibrary = await _context.AppLibraries
                .FirstOrDefaultAsync(al => al.UserId == userId && al.AppId == baseApp.AppId);

            result.Add(new UserLibraryItemDTO
            {
                AppId = baseApp.AppId,
                AppName = baseApp.AppName,
                Price = baseApp.Price,
                AppTypeId = baseApp.AppTypeId,
                HeaderImage = baseApp.AppImages
                                .Where(x => x.AppId == baseApp.AppId && x.ImageType == "header")
                                .Select(x => x.ImageUrl)
                                .FirstOrDefault(),
                PurchaseDate = appLibrary?.PurchaseDate ?? DateTime.MinValue,
                RelatedApps = relatedDtos
            });
        }

        return result;
    }

    /// <summary>
    /// Retrieves the user's purchase history, including wallet top-ups and game purchases.
    /// </summary>
    public async Task<List<PurchaseHistoryDTO>> GetUserPurchaseHistoryAsync(int userId)
    {
        var histories = await _context.PurchaseHistories
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.PurchaseDate)
            .ToListAsync();

        // Extract all distinct non-null AppIds
        var appIds = histories
            .Where(h => h.AppId.HasValue)
            .Select(h => h.AppId.Value)
            .Distinct()
            .ToList();

        var apps = await _context.Apps
            .Where(a => appIds.Contains(a.AppId))
            .Include(a => a.AppImages)
            .ToDictionaryAsync(a => a.AppId);

        var result = histories.Select(h =>
        {
            var app = h.AppId.HasValue && apps.TryGetValue(h.AppId.Value, out var foundApp) ? foundApp : null;

            return new PurchaseHistoryDTO
            {
                AppId = h.AppId,
                AppName = app?.AppName ?? "Wallet Top-Up",
                AppHeaderImage = app?.AppImages
                    .FirstOrDefault(i => i.ImageType == "header")?.ImageUrl,
                PaymentMethod = h.PaymentMethod,
                PriceAtPurchase = h.PriceAtPurchase,
                WalletChange = h.WalletChange,
                WalletBalanceAfter = h.WalletBalanceAfter,
                PurchaseDate = h.PurchaseDate
            };
        }).ToList();

        return result;
    }
}
