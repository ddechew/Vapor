namespace VaporWebAPI.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.IdentityModel.Tokens.Jwt;

using VaporWebAPI.DTOs;
using VaporWebAPI.Services;

/// <summary>
/// Manages user-created posts, likes, comments, and image uploads in the Community section.
/// Supports creating posts for owned apps, liking posts, and moderating comments.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PostController : ControllerBase
{
    private readonly PostService _postService;

    public PostController(PostService postService)
    {
        _postService = postService;
    }

    /// <summary>
    /// Retrieves all community posts with their like and comment counts.
    /// </summary>
    /// <returns>List of post previews.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAllPosts()
    {
        var posts = await _postService.GetAllPostsAsync();
        return Ok(posts);
    }

    /// <summary>
    /// Toggles the like status of a post for the authenticated user.
    /// </summary>
    /// <param name="postId">The ID of the post to like or unlike.</param>
    /// <returns>Status of the toggle operation.</returns>
    [HttpPost("{postId}/like")]
    [Authorize]
    public async Task<IActionResult> ToggleLikePost(int postId)
    {
        var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        if (username == null)
            return Unauthorized();

        var success = await _postService.ToggleLikePostAsync(username, postId);
        if (!success)
            return BadRequest("You already liked this post or post doesn't exist.");

        return Ok("Like toggled.");
    }

    /// <summary>
    /// Gets a list of users who liked a specific post.
    /// </summary>
    /// <param name="postId">Post ID to retrieve likers for.</param>
    [HttpGet("{postId}/likers")]
    public async Task<IActionResult> GetPostLikers(int postId)
    {
        var users = await _postService.GetPostLikersAsync(postId);
        return Ok(users);
    }

    /// <summary>
    /// Retrieves a list of post IDs liked by the authenticated user.
    /// </summary>
    [HttpGet("liked")]
    [Authorize]
    public async Task<IActionResult> GetUserLikedPosts()
    {
        var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        if (username == null)
            return Unauthorized();

        var likedPostIds = await _postService.GetUserLikedPostIdsAsync(username);
        return Ok(likedPostIds); 
    }

    /// <summary>
    /// Uploads an image for use in a post.
    /// </summary>
    /// <param name="file">The image file to upload.</param>
    /// <returns>URL of the uploaded image.</returns>
    [HttpPost("upload-image")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadPostImage(IFormFile file)
    {
        var imageUrl = await _postService.UploadPostImageAsync(file);
        if (imageUrl == null)
            return BadRequest("Image upload failed.");

        return Ok(new { imageUrl });
    }

    /// <summary>
    /// Creates a new post related to an owned app.
    /// </summary>
    /// <param name="dto">Post content and associated app ID.</param>
    /// <returns>Status message of the creation.</returns>
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreatePost([FromBody] CreatePostDTO dto)
    {
        var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        if (username == null)
            return Unauthorized();

        var result = await _postService.CreatePostAsync(username, dto);
        if (!result)
            return BadRequest("Could not create post. You might not own the app.");

        return Ok("✅ Post created successfully.");
    }

    /// <summary>
    /// Gets all comments under a specific post.
    /// </summary>
    /// <param name="postId">Post ID to retrieve comments for.</param>
    [HttpGet("{postId}/comments")]
    public async Task<IActionResult> GetComments(int postId)
    {
        var comments = await _postService.GetCommentsAsync(postId);
        return Ok(comments);
    }

    /// <summary>
    /// Adds a comment to a specific post.
    /// </summary>
    /// <param name="dto">Comment content and target post ID.</param>
    [HttpPost("comment")]
    [Authorize]
    public async Task<IActionResult> AddComment([FromBody] CommentRequestDTO dto)
    {
        var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        if (username == null) return Unauthorized();

        var success = await _postService.AddCommentAsync(username, dto);
        if (!success) return BadRequest("Failed to add comment");

        return Ok("✅ Comment posted");
    }

    /// <summary>
    /// Soft-deletes the user's own comment.
    /// </summary>
    /// <param name="commentId">ID of the comment to delete.</param>
    [HttpDelete("comment/{commentId}")]
    [Authorize]
    public async Task<IActionResult> DeleteComment(int commentId)
    {
        var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        if (username == null) return Unauthorized();

        var success = await _postService.SoftDeleteCommentAsync(commentId, username);
        if (!success) return BadRequest("Failed to delete comment or not your comment");

        return Ok("✅ Comment deleted");
    }

    /// <summary>
    /// Edits an existing comment by the user.
    /// </summary>
    /// <param name="commentId">ID of the comment to edit.</param>
    /// <param name="newText">The updated comment text.</param>
    [HttpPut("comment/{commentId}")]
    [Authorize]
    public async Task<IActionResult> EditComment(int commentId, [FromBody] string newText)
    {
        var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        if (username == null) return Unauthorized();

        var success = await _postService.EditCommentAsync(commentId, username, newText);
        if (!success) return BadRequest("Failed to edit comment or unauthorized.");

        return Ok("✅ Comment updated");
    }

    /// <summary>
    /// Deletes a post authored by the user.
    /// </summary>
    /// <param name="postId">ID of the post to delete.</param>
    [HttpDelete("{postId}")]
    [Authorize]
    public async Task<IActionResult> DeletePost(int postId)
    {
        var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        if (username == null) return Unauthorized();

        var success = await _postService.DeletePostAsync(postId, username);
        if (!success) return BadRequest("Failed to delete post");

        return Ok("✅ Post deleted");
    }

    /// <summary>
    /// Allows the post owner to delete a comment on their post.
    /// </summary>
    /// <param name="commentId">ID of the comment to soft-delete.</param>
    [HttpDelete("comment/by-owner/{commentId}")]
    [Authorize]
    public async Task<IActionResult> DeleteCommentByPostOwner(int commentId)
    {
        var username = User.FindFirst(JwtRegisteredClaimNames.Name)?.Value;
        if (username == null) return Unauthorized();

        var success = await _postService.SoftDeleteCommentByPostOwnerAsync(commentId, username);
        if (!success) return BadRequest("Failed to delete comment or it was already deleted.");

        return Ok("✅ Comment deleted by post owner");
    }
}
