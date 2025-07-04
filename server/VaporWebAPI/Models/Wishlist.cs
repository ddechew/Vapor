namespace VaporWebAPI.Models;

/// <summary>
/// Represents a wishlist entry where a user has added a particular app with a priority order.
/// Used to track which apps the user wishes to purchase or follow.
/// </summary>
public partial class Wishlist
{
    /// <summary>
    /// Primary key identifier for the wishlist entry.
    /// </summary>
    public int WishlistId { get; set; }

    /// <summary>
    /// Foreign key to the user who owns this wishlist entry.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Foreign key to the app that is added to the wishlist.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Priority order of this app in the user's wishlist. Lower numbers indicate higher priority.
    /// </summary>
    public int Priority { get; set; }

    /// <summary>
    /// Timestamp when this wishlist entry was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Timestamp of the last modification of this wishlist entry.
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the related App entity.
    /// </summary>
    public virtual App App { get; set; } = null!;

    /// <summary>
    /// Navigation property to the related User entity who owns this wishlist entry.
    /// </summary>
    public virtual User User { get; set; } = null!;
}
