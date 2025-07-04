namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object representing a wishlist entry for administrative purposes.
/// </summary>
public class WishlistAdminDTO
{
    /// <summary>
    /// Unique identifier of the wishlist entry.
    /// </summary>  
    public int WishlistId { get; set; }

    /// <summary>
    /// Identifier of the user who owns the wishlist entry; nullable if user is deleted.
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Display name of the user who owns the wishlist entry.
    /// </summary>
    public string UserDisplayName { get; set; }

    /// <summary>
    /// Identifier of the app added to the wishlist.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Name of the app added to the wishlist.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// Date and time when the app was added to the wishlist.
    /// </summary>
    public DateTime AddedAt { get; set; }
}
