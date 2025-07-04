namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object representing a like on a post for admin views.
/// </summary>
public class PostLikeAdminDTO
{
    /// <summary>
    /// Unique identifier of the like.
    /// </summary>
    public int LikeId { get; set; }

    /// <summary>
    /// Identifier of the post that was liked.
    /// </summary>
    public int PostId { get; set; }

    /// <summary>
    /// Content of the liked post.
    /// </summary>
    public string PostContent { get; set; }

    /// <summary>
    /// Identifier of the user who liked the post (nullable).
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Display name of the user who liked the post.
    /// </summary>
    public string UserDisplayName { get; set; }

    /// <summary>
    /// Date and time when the like was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
