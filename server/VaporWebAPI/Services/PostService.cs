namespace VaporWebAPI.Services;

using Microsoft.EntityFrameworkCore;

using VaporWebAPI.Data;
using VaporWebAPI.DTOs;
using VaporWebAPI.Models;

public class PostService
{
    private readonly VaporDbContext _context;
    private readonly NotificationService _notificationService;

    public PostService(VaporDbContext context, NotificationService notificationService)
    {
        _context = context;
        _notificationService = notificationService;
    }

    /// <summary>
    /// Retrieves all posts with their associated app info, likes, and comment counts.
    /// </summary>
    public async Task<List<PostResponseDTO>> GetAllPostsAsync()
    {
        return await _context.Posts
            .Include(p => p.App)
                .ThenInclude(a => a.AppImages)
            .Include(p => p.PostLikes)
            .Include(p => p.PostComments)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PostResponseDTO
            {
                PostId = p.PostId,
                Content = p.Content,
                CreatedAt = p.CreatedAt,
                ImageUrl = p.ImageUrl,
                UserDisplayName = p.UserDisplayName,
                Username = p.User != null ? p.User.Username : null,
                Avatar = p.User != null ? p.User.ProfilePicture : "/assets/defaultVaporProfilePic.jpg",
                AppId = p.AppId,
                AppName = p.App.AppName,
                HeaderImage = p.App.AppImages
                    .Where(i => i.ImageType == "header")
                    .Select(i => i.ImageUrl)
                    .FirstOrDefault(),
                LikesCount = p.PostLikes.Count,
                CommentsCount = p.PostComments.Count
            })
            .ToListAsync();
    }

    /// <summary>
    /// Toggles a like on a post by the given user. If already liked, removes the like; otherwise adds it and notifies the post owner.
    /// </summary>
    /// <param name="username">The username of the user liking the post.</param>
    /// <param name="postId">The ID of the post to like.</param>
    public async Task<bool> ToggleLikePostAsync(string username, int postId)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return false;

        var post = await _context.Posts.FindAsync(postId); 

        if (post == null) return false;

        var existingLike = await _context.PostLikes
            .FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == user.UserId);

        if (existingLike != null)
        {
            _context.PostLikes.Remove(existingLike);
        }
        else
        {
            _context.PostLikes.Add(new PostLike
            {
                PostId = postId,
                UserId = user.UserId
            });

            if (post.UserId.HasValue && post.UserId != user.UserId)
            {
                await _notificationService.CreateNotificationAsync(
                    userId: post.UserId.Value,
                    senderId: user.UserId,
                    message: $"{user.DisplayName} liked your post.",
                    postId: postId
                );
            }
        }

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Retrieves a list of users who liked a specific post.
    /// </summary>
    /// <param name="postId">The ID of the post.</param>
    public async Task<List<UserLikeDTO>> GetPostLikersAsync(int postId)
    {
        return await _context.PostLikes
            .Where(like => like.PostId == postId && like.User != null)
            .Include(like => like.User)
            .Select(like => new UserLikeDTO
            {
                Username = like.User.Username,
                DisplayName = like.User.DisplayName ?? like.User.Username ?? "[deleted]",
                ProfilePicture = like.User.ProfilePicture ?? "/assets/defaultVaporProfilePic.jpg"
            })
            .ToListAsync(); 
    }

    /// <summary>
    /// Returns a list of post IDs that the specified user has liked.
    /// </summary>
    /// <param name="username">The user's username.</param>
    public async Task<List<int>> GetUserLikedPostIdsAsync(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return new List<int>();

        return await _context.PostLikes
            .Where(pl => pl.UserId == user.UserId)
            .Select(pl => pl.PostId)
            .ToListAsync();
    }

    /// <summary>
    /// Creates a new post for an app, if the user owns the app.
    /// </summary>
    /// <param name="username">The author of the post.</param>
    /// <param name="dto">Post creation data (appId, content, imageUrl).</param>
    public async Task<bool> CreatePostAsync(string username, CreatePostDTO dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null)
            return false;

        var ownsApp = await _context.AppLibraries.AnyAsync(a => a.UserId == user.UserId && a.AppId == dto.AppId);
        if (!ownsApp)
            return false;

        var post = new Post
        {
            UserId = user.UserId,
            AppId = dto.AppId,
            Content = dto.Content,
            CreatedAt = DateTime.UtcNow,
            UserDisplayName = user.DisplayName ?? user.Username,
            ImageUrl = dto.ImageUrl
        };

        _context.Posts.Add(post);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Uploads an image file for use in a post and saves it to the public assets folder.
    /// </summary>
    /// <param name="file">The image file to upload.</param>
    /// <returns>Relative URL path to the uploaded image, or null if failed.</returns>
    public async Task<string?> UploadPostImageAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return null;

        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var relativePath = Path.Combine("assets", fileName);
        var clientPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "client", "public", relativePath);

        Directory.CreateDirectory(Path.GetDirectoryName(clientPath)!);

        using (var stream = new FileStream(clientPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return $"/assets/{fileName}";
    }

    /// <summary>
    /// Deletes a post made by the user.
    /// </summary>
    /// <param name="postId">The ID of the post to delete.</param>
    /// <param name="username">The username of the post author.</param>
    public async Task<bool> DeletePostAsync(int postId, string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return false;

        var post = await _context.Posts.FirstOrDefaultAsync(p => p.PostId == postId && p.UserId == user.UserId);
        if (post == null) return false;

        _context.Posts.Remove(post);
        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Edits an existing comment by the comment's author.
    /// </summary>
    /// <param name="commentId">The ID of the comment to edit.</param>
    /// <param name="username">The user making the edit.</param>
    /// <param name="newText">The updated comment text.</param>
    public async Task<bool> EditCommentAsync(int commentId, string username, string newText)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return false;

        var comment = await _context.PostComments
            .FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == user.UserId);

        if (comment == null) return false;
        if (comment.CommentText.Trim() == newText.Trim()) return false;

        comment.CommentText = newText;
        comment.IsEdited = true;
        comment.LastModification21180128 = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Allows a post owner to soft-delete someone else’s comment on their post, marking it as deleted and notifying the commenter.
    /// </summary>
    /// <param name="commentId">The ID of the comment.</param>
    /// <param name="username">The post owner's username.</param>
    public async Task<bool> SoftDeleteCommentByPostOwnerAsync(int commentId, string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return false;

        var comment = await _context.PostComments
            .Include(c => c.Post)
            .FirstOrDefaultAsync(c => c.CommentId == commentId);

        if (comment == null || comment.Post.UserId != user.UserId) return false;

        if (comment.UserDisplayName == "[deleted]") return false;

        comment.UserDisplayName = "[deleted]";
        comment.CommentText = "Comment deleted by post owner";

        if (comment.UserId.HasValue && comment.UserId != user.UserId)
        {
            await _notificationService.CreateNotificationAsync(
                userId: comment.UserId.Value,
                senderId: user.UserId,
                message: $"{user.DisplayName} deleted your comment from their post.",
                postId: comment.PostId
            );
        }

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Retrieves all comments for a given post, ordered by creation date (newest first).
    /// </summary>
    /// <param name="postId">The ID of the post.</param>
    public async Task<List<PostCommentDTO>> GetCommentsAsync(int postId)
    {
        return await _context.PostComments
            .Where(c => c.PostId == postId)
            .Include(c => c.User)
            .OrderByDescending(c => c.CreatedAt)
            .Select(c => new PostCommentDTO
            {
                CommentId = c.CommentId,
                Username = c.User.Username,
                DisplayName = c.UserDisplayName,
                ProfilePicture = c.User.ProfilePicture,
                CommentText = c.CommentText,
                CreatedAt = c.CreatedAt,
                IsEdited = c.IsEdited
            })
            .ToListAsync();
    }

    /// <summary>
    /// Adds a new comment to a post and notifies the post owner.
    /// </summary>
    /// <param name="username">The author of the comment.</param>
    /// <param name="dto">The comment data including text and postId.</param>
    public async Task<bool> AddCommentAsync(string username, CommentRequestDTO dto)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return false;

        var post = await _context.Posts.FindAsync(dto.PostId);
        if (post == null) return false;

        _context.PostComments.Add(new PostComment
        {
            PostId = dto.PostId,
            UserDisplayName = user.DisplayName,
            UserId = user.UserId,
            CommentText = dto.CommentText,
            CreatedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();

        if (post.UserId.HasValue && post.UserId != user.UserId)
        {
            await _notificationService.CreateNotificationAsync(
                userId: post.UserId.Value,
                senderId: user.UserId,
                message: $"{user.DisplayName} commented on your post.",
                postId: dto.PostId
            );
        }

        return true;
    }

    /// <summary>
    /// Allows a user to soft-delete their own comment by replacing its content and display name.
    /// </summary>
    /// <param name="commentId">The ID of the comment.</param>
    /// <param name="username">The user requesting deletion.</param>
    public async Task<bool> SoftDeleteCommentAsync(int commentId, string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        if (user == null) return false;

        var comment = await _context.PostComments
            .Include(c => c.User)
            .FirstOrDefaultAsync(c => c.CommentId == commentId && c.UserId == user.UserId);

        if (comment == null) return false;

        if (comment.UserDisplayName == "[deleted]") return false;

        comment.UserDisplayName = "[deleted]";
        comment.CommentText = "Comment deleted by user";

        await _context.SaveChangesAsync();
        return true;
    }
}
