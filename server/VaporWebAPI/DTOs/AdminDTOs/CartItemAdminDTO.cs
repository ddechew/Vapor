namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object representing a cart item in the admin panel.
/// </summary>
public class CartItemAdminDTO
{
    /// <summary>
    /// Unique identifier of the cart item.
    /// </summary>
    public int CartItemId { get; set; }

    /// <summary>
    /// Identifier of the user who owns the cart.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Display name of the user (nullable if user is deleted).
    /// </summary>
    public string? UserDisplayName { get; set; }

    /// <summary>
    /// Identifier of the application added to the cart.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Name of the application.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// Date and time when the item was added to the cart (nullable).
    /// </summary>
    public DateTime? AddedAt { get; set; }
}
