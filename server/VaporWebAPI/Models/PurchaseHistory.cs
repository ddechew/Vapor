namespace VaporWebAPI.Models;

/// <summary>
/// Represents a record of a purchase transaction made by a user.
/// </summary>
public partial class PurchaseHistory
{
    /// <summary>
    /// Primary key identifier for the purchase record.
    /// </summary>
    public int PurchaseId { get; set; }

    /// <summary>
    /// The ID of the user who made the purchase.
    /// </summary>  
    public int UserId { get; set; }

    /// <summary>
    /// The ID of the purchased app, if applicable (null for wallet top-ups).
    /// </summary>
    public int? AppId { get; set; }

    /// <summary>
    /// The date and time when the purchase was made.
    /// </summary>
    public DateTime PurchaseDate { get; set; }

    /// <summary>
    /// The price at which the app was purchased.
    /// </summary>
    public decimal PriceAtPurchase { get; set; }

    /// <summary>
    /// The payment method used for the purchase (e.g., "Wallet", "Stripe", "Free").
    /// </summary>
    public string PaymentMethod { get; set; } = null!;

    /// <summary>
    /// The change in the user's wallet balance resulting from this purchase (negative for purchases).
    /// </summary>
    public decimal? WalletChange { get; set; }

    /// <summary>
    /// The user's wallet balance after the purchase was completed.
    /// </summary>
    public decimal? WalletBalanceAfter { get; set; }

    /// <summary>
    /// Timestamp of the last modification for this record.
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the user who made the purchase.
    /// </summary>
    public virtual User User { get; set; } = null!;
}
