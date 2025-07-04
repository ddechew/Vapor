namespace VaporWebAPI.Services;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

using ClosedXML.Excel;

using VaporWebAPI.Data;
using VaporWebAPI.DTOs.AdminDTOs;
using VaporWebAPI.Models;
using VaporWebAPI.Utils;


public class AdminService
{
    private readonly VaporDbContext _context;
    private readonly PasswordHasher<string> _passwordHasher;

    public AdminService(VaporDbContext context)
    {
        _context = context;
        _passwordHasher = new PasswordHasher<string>();
    }

    //===================================================================================================================================
    // == USER ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves a list of all users along with their roles.
    /// </summary>
    /// <returns></returns>
    public async Task<List<UserAdminDTO>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Role)
            .Select(u => new UserAdminDTO
            {
                UserId = u.UserId,
                Username = u.Username,
                DisplayName = u.DisplayName,
                Email = u.Email,
                Wallet = u.Wallet,
                Points = u.Points,
                RoleId = u.RoleId,
                RoleName = u.Role.RoleName
            })
            .ToListAsync();
    }

    /// <summary>
    /// Generates an Excel file containing user information.
    /// </summary>
    public async Task<byte[]> GenerateUsersExcelAsync()
    {
        var users = await _context.Users.ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Users");

        worksheet.Cell(1, 1).Value = "UserId";
        worksheet.Cell(1, 2).Value = "Username";
        worksheet.Cell(1, 3).Value = "DisplayName";
        worksheet.Cell(1, 4).Value = "Email";
        worksheet.Cell(1, 5).Value = "Wallet";
        worksheet.Cell(1, 6).Value = "Points";
        worksheet.Cell(1, 7).Value = "Role";

        for (int i = 0; i < users.Count; i++)
        {
            var u = users[i];
            worksheet.Cell(i + 2, 1).Value = u.UserId;
            worksheet.Cell(i + 2, 2).Value = u.Username;
            worksheet.Cell(i + 2, 3).Value = u.DisplayName;
            worksheet.Cell(i + 2, 4).Value = u.Email;
            worksheet.Cell(i + 2, 5).Value = u.Wallet;
            worksheet.Cell(i + 2, 6).Value = u.Points;
            worksheet.Cell(i + 2, 7).Value = u.RoleId == 2 ? "Admin" : "User";
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    /// <summary>
    /// Updates an existing user's details, including role, username, email, and display name.
    /// </summary>
    /// <param name="actorId">The ID of the admin performing the update.</param>
    /// <param name="updatedUser">The updated user data.</param>
    public async Task<bool> UpdateUserAsync(string actorId, UserAdminDTO updatedUser)
    {

        var user = await _context.Users.FindAsync(updatedUser.UserId);
        if (user == null) return false;

        if (user.Username != updatedUser.Username)
        {
            var usernameValidation = InputValidation.ValidateUsername(updatedUser.Username);
            if (usernameValidation != "Valid") throw new Exception(usernameValidation);

            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username == updatedUser.Username && u.UserId != user.UserId);
            if (usernameExists) throw new Exception("Username is already taken.");

            user.Username = updatedUser.Username;
        }

        if (user.Email != updatedUser.Email)
        {
            if (!new EmailAddressAttribute().IsValid(updatedUser.Email))
                throw new Exception("Invalid email address.");

            var emailExists = await _context.Users
                .AnyAsync(u => u.Email == updatedUser.Email && u.UserId != user.UserId);
            if (emailExists) throw new Exception("Email is already in use.");

            user.Email = updatedUser.Email;
        }

        if (user.DisplayName != updatedUser.DisplayName)
        {
            user.DisplayName = updatedUser.DisplayName;

            await _context.PostComments
                .Where(c => c.UserId == user.UserId && c.UserDisplayName != "[deleted]")
                .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.UserDisplayName, user.DisplayName));

            await _context.Posts
                .Where(c => c.UserId == user.UserId && c.UserDisplayName != "[deleted]")
                .ExecuteUpdateAsync(setters => setters.SetProperty(c => c.UserDisplayName, user.DisplayName));
        }

        if (user.Wallet != updatedUser.Wallet)
            user.Wallet = updatedUser.Wallet;

        if (user.Points != updatedUser.Points)
            user.Points = updatedUser.Points;

        if (user.RoleId != updatedUser.RoleId)
            user.RoleId = updatedUser.RoleId;

        using var transaction = await _context.Database.BeginTransactionAsync();
        var connection = _context.Database.GetDbConnection();

        await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_set_session_context @key = N'ModifiedBy', @value = {0}",
           $"UserId: {actorId}"
        );

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return true;
    }

    /// <summary>
    /// Creates a new user after validating all input fields.
    /// </summary>
    /// <param name="actorId">The ID of the admin creating the user.</param>
    /// <param name="newUser">The new user details including password.</param>
    public async Task<UserAdminDTO> CreateUserAsync(string actorId, UserAdminDTO newUser)
    {
        var usernameValidation = InputValidation.ValidateUsername(newUser.Username);
        if (usernameValidation != "Valid")
            throw new Exception(usernameValidation);

        if (!new EmailAddressAttribute().IsValid(newUser.Email))
            throw new Exception("Invalid email address.");

        var usernameExists = await _context.Users.AnyAsync(u => u.Username == newUser.Username);
        if (usernameExists)
            throw new Exception("Username already taken.");

        var emailExists = await _context.Users.AnyAsync(u => u.Email == newUser.Email);
        if (emailExists)
            throw new Exception("Email already in use.");

        var roleExists = await _context.Roles.AnyAsync(r => r.RoleId == newUser.RoleId);
        if (!roleExists)
            throw new Exception("Role not found.");

        if (string.IsNullOrWhiteSpace(newUser.Password))
            throw new Exception("Password is required.");

        var passwordFeedback = InputValidation.ValidatePassword(newUser.Password);
        if (passwordFeedback != "Valid")
            throw new Exception(passwordFeedback);

        var user = new User
        {
            Username = newUser.Username,
            DisplayName = newUser.DisplayName,
            Email = newUser.Email,
            Wallet = newUser.Wallet,
            Points = newUser.Points,
            RoleId = newUser.RoleId,
            LastModification21180128 = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(null, newUser.Password);

        using var transaction = await _context.Database.BeginTransactionAsync();
        var connection = _context.Database.GetDbConnection();

        await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_set_session_context @key = N'ModifiedBy', @value = {0}",
            $"UserId: {actorId}"
        );

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return new UserAdminDTO
        {
            UserId = user.UserId,
            Username = user.Username,
            DisplayName = user.DisplayName,
            Email = user.Email,
            Wallet = user.Wallet,
            Points = user.Points,
            RoleId = user.RoleId,
            RoleName = (await _context.Roles.FindAsync(user.RoleId))?.RoleName ?? "Unknown"
        };
    }

    /// <summary>
    /// Deletes a user by ID unless the actor is trying to delete themselves.
    /// </summary>
    /// <param name="actorId">The ID of the acting admin.</param>
    /// <param name="id">The ID of the user to delete.</param>
    public async Task<bool> DeleteUserAsync(int actorId, int id)
    {
        if (actorId == id)
            return false;

        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return false;

        await NullifyNotificationSendersAsync(user.UserId);

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    //===================================================================================================================================
    // == ROLES ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all available roles from the database.
    /// </summary>
    public async Task<List<Role>> GetAllRolesAsync()
    {
        return await _context.Roles.ToListAsync();
    }

    /// <summary>
    /// Updates a role's name.
    /// </summary>
    /// <param name="actorId">The ID of the admin updating the role.</param>
    /// <param name="id">The ID of the role to update.</param>
    /// <param name="updatedRole">The updated role data.</param>
    public async Task<bool> UpdateRoleAsync(string actorId, int id, Role updatedRole)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return false;

        using var transaction = await _context.Database.BeginTransactionAsync();

        await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_set_session_context @key = N'ModifiedBy', @value = {0}",
            $"UserId: {actorId}"
        );

        role.RoleName = updatedRole.RoleName;
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return true;
    }

    /// <summary>
    /// Creates a new role in the system.
    /// </summary>
    /// <param name="actorId">The ID of the admin creating the role.</param>
    /// <param name="newRole">The role data to create.</param>
    public async Task<Role> CreateRoleAsync(string actorId, Role newRole)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();

        await _context.Database.ExecuteSqlRawAsync(
            "EXEC sp_set_session_context @key = N'ModifiedBy', @value = {0}",
            $"UserId: {actorId}"
        );

        _context.Roles.Add(newRole);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return newRole;
    }

    /// <summary>
    /// Deletes a role by ID.
    /// </summary>
    /// <param name="id">The ID of the role to delete.</param>
    public async Task<bool> DeleteRoleAsync(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role == null) return false;

        _context.Roles.Remove(role);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Generates an Excel file listing all roles.
    /// </summary>
    public async Task<byte[]> GenerateRolesExcelAsync()
    {
        var roles = await _context.Roles.ToListAsync();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Roles");

        worksheet.Cell(1, 1).Value = "RoleId";
        worksheet.Cell(1, 2).Value = "RoleName";

        for (int i = 0; i < roles.Count; i++)
        {
            var r = roles[i];
            worksheet.Cell(i + 2, 1).Value = r.RoleId;
            worksheet.Cell(i + 2, 2).Value = r.RoleName;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    //===================================================================================================================================
    // == DEVELOPERS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all developers from the system.
    /// </summary>
    public async Task<List<Developer>> GetAllDevelopersAsync()
    {
        return await _context.Developers.ToListAsync();
    }

    /// <summary>
    /// Creates a new developer entry.
    /// </summary>
    /// <param name="newDev">The developer data to create.</param>
    public async Task<Developer> CreateDeveloperAsync(Developer newDev)
    {
        _context.Developers.Add(newDev);
        await _context.SaveChangesAsync();
        return newDev;
    }

    /// <summary>
    /// Updates the name of an existing developer.
    /// </summary>
    /// <param name="id">The ID of the developer to update.</param>
    /// <param name="updated">The updated developer data.</param>
    public async Task<bool> UpdateDeveloperAsync(int id, Developer updated)
    {
        var dev = await _context.Developers.FindAsync(id);
        if (dev == null) return false;

        dev.DeveloperName = updated.DeveloperName;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Deletes a developer by ID.
    /// </summary>
    /// <param name="id">The ID of the developer to delete.</param>
    public async Task<bool> DeleteDeveloperAsync(int id)
    {
        var dev = await _context.Developers.FindAsync(id);
        if (dev == null) return false;

        _context.Developers.Remove(dev);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Generates an Excel file listing all developers.
    /// </summary>
    public async Task<byte[]> GenerateDevelopersExcelAsync()
    {
        var devs = await _context.Developers.ToListAsync();
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Developers");

        worksheet.Cell(1, 1).Value = "DeveloperId";
        worksheet.Cell(1, 2).Value = "DeveloperName";

        for (int i = 0; i < devs.Count; i++)
        {
            worksheet.Cell(i + 2, 1).Value = devs[i].DeveloperId;
            worksheet.Cell(i + 2, 2).Value = devs[i].DeveloperName;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    /// <summary>
    /// Removes the sender reference from all notifications sent by a deleted user.
    /// </summary>
    /// <param name="userId">The ID of the user whose notifications will be anonymized.</param>
    private async Task NullifyNotificationSendersAsync(int userId)
    {
        var notifications = await _context.Notifications
            .Where(n => n.SenderId == userId)
            .ToListAsync();

        foreach (var n in notifications)
        {
            n.SenderId = null;
        }

        await _context.SaveChangesAsync();
    }

    //===================================================================================================================================
    // == APP TYPES ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all AppType records from the database asynchronously.
    /// </summary>
    /// <returns>A list of all AppType entities.</returns>
    public async Task<List<AppType>> GetAllAppTypesAsync()
    {
        return await _context.AppTypes.ToListAsync();
    }

    /// <summary>
    /// Creates a new AppType record in the database asynchronously.
    /// </summary>
    /// <param name="newType">The new AppType entity to add.</param>
    /// <returns>The created AppType entity with its database-assigned properties.</returns>
    public async Task<AppType> CreateAppTypeAsync(AppType newType)
    {
        _context.AppTypes.Add(newType);
        await _context.SaveChangesAsync();
        return newType;
    }

    /// <summary>
    /// Updates an existing AppType record identified by id with the provided updated data asynchronously.
    /// </summary>
    /// <param name="id">The id of the AppType to update.</param>
    /// <param name="updated">An AppType entity containing updated properties.</param>
    /// <returns>True if update succeeded; false if no AppType with the specified id was found.</returns>
    public async Task<bool> UpdateAppTypeAsync(int id, AppType updated)
    {
        var type = await _context.AppTypes.FindAsync(id);
        if (type == null) return false;

        type.TypeName = updated.TypeName;
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Deletes the AppType record identified by id asynchronously.
    /// </summary>
    /// <param name="id">The id of the AppType to delete.</param>
    /// <returns>True if deletion succeeded; false if no AppType with the specified id was found.</returns>
    public async Task<bool> DeleteAppTypeAsync(int id)
    {
        var type = await _context.AppTypes.FindAsync(id);
        if (type == null) return false;

        _context.AppTypes.Remove(type);
        await _context.SaveChangesAsync();
        return true;
    }

    //===================================================================================================================================
    // == APPS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all app types from the database asynchronously.
    /// </summary>
    public async Task<List<AppAdminDTO>> GetAllAppsAsync()
    {
        return await _context.Apps
            .Include(a => a.AppType)
            .Select(a => new AppAdminDTO
            {
                AppId = a.AppId,
                Name = a.AppName,
                Price = a.Price ?? 0,
                Description = a.Description,
                AppTypeId = a.AppTypeId,
                AppTypeName = a.AppType.TypeName
            })
            .OrderByDescending(a => a.AppId)
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new app type and saves it to the database asynchronously.
    /// </summary>
    public async Task<App> CreateAppAsync(AppCreateDTO dto)
    {
        bool nameExists = await _context.Apps
            .AnyAsync(a => a.AppName.ToLower() == dto.AppName.ToLower());

        if (nameExists)
            throw new InvalidOperationException("An app with this name already exists.");

        if (dto.Price < 0)
            throw new InvalidOperationException("Price cannot be negative.");

        int nextId = (_context.Apps.Any() ? _context.Apps.Max(a => a.AppId) : 0) + 1;

        var app = new App
        {
            AppId = nextId,
            AppName = dto.AppName,
            Description = dto.Description,
            Price = dto.Price,
            AppTypeId = dto.AppTypeId,
            BaseAppId = dto.BaseAppId,
            ReleaseDate = DateTime.UtcNow.ToString(),
            PurchaseCount = 0,
            LastModification21180128 = DateTime.UtcNow
        };

        _context.Apps.Add(app);
        await _context.SaveChangesAsync();
        return app;
    }

    /// <summary>
    /// Updates an existing app type by ID asynchronously.
    /// </summary>
    public async Task<AppAdminDTO?> UpdateAppAsync(int appId, AppAdminDTO dto)
    {
        var existing = await _context.Apps.FindAsync(appId);
        if (existing == null) return null;

        existing.AppName = dto.Name;
        existing.Price = dto.Price;
        existing.Description = dto.Description;
        existing.AppTypeId = dto.AppTypeId;

        await _context.SaveChangesAsync();

        return new AppAdminDTO
        {
            AppId = existing.AppId,
            Name = existing.AppName,
            Price = existing.Price ?? 0,
            Description = existing.Description,
            AppTypeId = existing.AppTypeId,
            AppTypeName = _context.AppTypes.FirstOrDefault(a => a.AppTypeId == dto.AppTypeId)?.TypeName ?? "Unknown"
        };
    }

    /// <summary>
    /// Deletes an app type by ID asynchronously.
    /// </summary>
    public async Task<bool> DeleteAppAsync(int appId)
    {
        var app = await _context.Apps.FindAsync(appId);
        if (app == null) return false;

        _context.Apps.Remove(app);
        await _context.SaveChangesAsync();
        return true;
    }

    //===================================================================================================================================
    // == GENRES ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all genres asynchronously for admin display.
    /// </summary>
    public async Task<List<GenreAdminDTO>> GetAllGenresAsync()
    {
        return await _context.Genres
            .Select(g => new GenreAdminDTO
            {
                GenreId = g.GenreId,
                Name = g.GenreName
            })
            .OrderBy(g => g.GenreId)
            .ToListAsync();
    }

    //===================================================================================================================================
    // == PUBLISHERS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all publishers for admin view.
    /// </summary>
    public async Task<List<PublisherAdminDTO>> GetPublishersAsync()
    {
        return await _context.Publishers
            .Select(p => new PublisherAdminDTO
            {
                PublisherId = p.PublisherId,
                PublisherName = p.PublisherName
            })
            .OrderBy(p => p.PublisherId)
            .ToListAsync();
    }

    //===================================================================================================================================
    // == POSTS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all posts with related app and user info for admin.
    /// </summary>
    public async Task<List<PostAdminDTO>> GetAllPostsAsync()
    {
        return await _context.Posts
            .Include(p => p.App)
            .Include(p => p.User)
            .Select(p => new PostAdminDTO
            {
                PostId = p.PostId,
                AppId = p.AppId,
                AppName = p.App.AppName,
                UserId = p.UserId,
                UserDisplayName = p.User.DisplayName,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                LikesCount = p.PostLikes.Count(),
                CommentsCount = p.PostComments.Count()
            })
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    //===================================================================================================================================
    // == POST COMMENTS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all post comments with related post and user info for admin.
    /// </summary>
    public async Task<List<CommentAdminDTO>> GetAllCommentsAsync()
    {
        return await _context.PostComments
            .Include(c => c.Post)
            .Include(c => c.User)
            .Select(c => new CommentAdminDTO
            {
                CommentId = c.CommentId,
                PostId = c.PostId,
                PostContent = c.Post.Content,
                UserId = c.UserId,
                UserDisplayName = c.User.DisplayName,
                Content = c.CommentText,
                CreatedAt = c.CreatedAt
            })
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    //===================================================================================================================================
    // == POST LIKES ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all post likes with related post and user info for admin.
    /// </summary>
    public async Task<List<PostLikeAdminDTO>> GetAllPostLikesAsync()
    {
        return await _context.PostLikes
            .Include(l => l.Post)
            .Include(l => l.User)
            .Select(l => new PostLikeAdminDTO
            {
                LikeId = l.LikeId,
                PostId = l.PostId,
                PostContent = l.Post.Content,
                UserId = l.UserId,
                UserDisplayName = l.User.DisplayName,
                CreatedAt = l.LastModification21180128
            })
            .OrderByDescending(l => l.CreatedAt)
            .ToListAsync();
    }

    //===================================================================================================================================
    // == APP REVIEWS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all app reviews with related app and user info for admin.
    /// </summary>
    public async Task<List<AppReviewAdminDTO>> GetAllAppReviewsAsync()
    {
        return await _context.AppReviews
            .Include(r => r.App)
            .Include(r => r.User)
            .Select(r => new AppReviewAdminDTO
            {
                ReviewId = r.ReviewId,
                AppId = r.AppId,
                AppName = r.App.AppName,
                UserId = r.UserId,
                UserDisplayName = r.User != null ? r.User.DisplayName : "Deleted User",
                IsRecommended = r.IsRecommended,
                ReviewText = r.ReviewText,
                CreatedAt = r.CreatedAt,
                IsEdited = r.IsEdited
            })
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }


    //===================================================================================================================================
    // == APP IMAGES ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all app images with related app info for admin.
    /// </summary>
    public async Task<List<AppImageAdminDTO>> GetAllAppImagesAsync()
    {
        return await _context.AppImages
            .Include(i => i.App)
            .Select(i => new AppImageAdminDTO
            {
                ImageId = i.ImageId,
                AppId = i.AppId,
                AppName = i.App.AppName,
                ImageUrl = i.ImageUrl,
                AddedAt = i.LastModification21180128
            })
            .OrderByDescending(i => i.AddedAt)
            .ToListAsync();
    }

    //===================================================================================================================================
    // == APP VIDEOS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all app videos for admin view.
    /// </summary>
    public async Task<List<AppVideoAdminDTO>> GetAllAppVideosAsync()
    {
        return await _context.AppVideos
            .Include(v => v.App)
            .Select(v => new AppVideoAdminDTO
            {
                VideoId = v.VideoId,
                AppId = v.AppId,
                AppName = v.App.AppName,
                VideoUrl = v.VideoUrl,
                AddedAt = v.LastModification21180128
            })
            .OrderByDescending(v => v.AddedAt)
            .ToListAsync();
    }

    //===================================================================================================================================
    // == CART ITEMS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all cart items for admin view.
    /// </summary>
    public async Task<List<CartItemAdminDTO>> GetAllCartItemsAsync()
    {
        return await _context.CartItems
            .Include(ci => ci.User)
            .Include(ci => ci.App)
            .Select(ci => new CartItemAdminDTO
            {
                CartItemId = ci.CartItemId,
                UserId = ci.UserId,
                UserDisplayName = ci.User.DisplayName,
                AppId = ci.AppId,
                AppName = ci.App.AppName,
                AddedAt = ci.DateAdded
            })
            .OrderByDescending(ci => ci.AddedAt)
            .ToListAsync();
    }

    //===================================================================================================================================
    // == WISHLIST ITEMS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all wishlist items for admin view.
    /// </summary>
    public async Task<List<WishlistAdminDTO>> GetAllWishlistItemsAsync()
    {
        return await _context.Wishlists
            .Include(w => w.User)
            .Include(w => w.App)
            .Select(w => new WishlistAdminDTO
            {
                WishlistId = w.WishlistId,
                UserId = w.UserId,
                UserDisplayName = w.User != null ? w.User.DisplayName : "Deleted User",
                AppId = w.AppId,
                AppName = w.App.AppName,
                AddedAt = w.CreatedAt
            })
            .OrderByDescending(w => w.AddedAt)
            .ToListAsync();
    }

    //===================================================================================================================================
    // == PURCHASE HISTORY ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves purchase history entries for admin view.
    /// </summary>
    public async Task<List<PurchaseHistoryAdminDTO>> GetPurchaseHistoryAsync()
    {
        return await _context.AppLibraries
            .Include(al => al.App)
            .Include(al => al.User)
            .Select(al => new PurchaseHistoryAdminDTO
            {
                AppLibraryId = al.LibraryId,
                AppId = al.AppId,
                AppName = al.App.AppName,
                UserId = al.UserId,
                UserDisplayName = al.User != null ? al.User.DisplayName : null,
                PurchaseDate = al.PurchaseDate
            })
            .OrderByDescending(al => al.PurchaseDate)
            .ToListAsync();
    }

    //===================================================================================================================================
    // == NOTIFICATIONS ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves all notifications for admin view.
    /// </summary>
    public async Task<List<NotificationAdminDTO>> GetAllNotificationsAsync()
    {
        return await _context.Notifications
            .Include(n => n.User)
            .Select(n => new NotificationAdminDTO
            {
                NotificationId = n.NotificationId,
                UserId = n.UserId,
                UserDisplayName = n.User != null ? n.User.DisplayName : null,
                Message = n.Message,
                Timestamp = n.LastModification21180128
            })
            .OrderByDescending(n => n.Timestamp)
            .ToListAsync();
    }

    //===================================================================================================================================
    // == USER LIBRARIES ==
    //===================================================================================================================================

    /// <summary>
    /// Retrieves user libraries for admin view.
    /// </summary>
    public async Task<List<UserLibraryAdminDTO>> GetUserLibrariesAsync()
    {
        return await _context.AppLibraries
            .Include(lib => lib.User)
            .Include(lib => lib.App)
            .OrderByDescending(lib => lib.PurchaseDate)
            .Select(lib => new UserLibraryAdminDTO
            {
                LibraryId = lib.LibraryId,
                UserId = lib.UserId,
                UserDisplayName = lib.User != null ? lib.User.DisplayName : null,
                AppId = lib.AppId,
                AppName = lib.App.AppName,
                PurchaseDate = lib.PurchaseDate
            })
            .OrderByDescending(lib => lib.PurchaseDate)
            .ToListAsync();
    }



}
