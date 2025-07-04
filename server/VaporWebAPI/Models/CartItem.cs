namespace VaporWebAPI.Models;

/// <summary>
/// Represents an app added to a user's shopping cart.
/// </summary>
public partial class CartItem
{
    /// <summary>
    /// Primary key for the cart item entry.
    /// </summary>
    public int CartItemId { get; set; }

    /// <summary>
    /// Foreign key referencing the user who added the item.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Foreign key referencing the app added to the cart.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Quantity of the app in the cart (optional, typically 1 for digital products).
    /// </summary>
    public int? Quantity { get; set; }

    /// <summary>
    /// Date and time the app was added to the cart.
    /// </summary>
    public DateTime? DateAdded { get; set; }

    /// <summary>
    /// Timestamp of the last modification (suffix required: 21180128).
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the associated app.
    /// </summary>
    public virtual App App { get; set; } = null!;

    /// <summary>
    /// Navigation property to the associated user.
    /// </summary>
    public virtual User User { get; set; } = null!;
}
