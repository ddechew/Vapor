namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents the full invoice model used to generate purchase receipts for users,
/// including itemized purchases, discount information, and user details.
/// </summary>
public class PurchaseInvoiceModel
{
    /// <summary>
    /// The username of the user who made the purchase.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// The UTC date and time when the purchase was made.
    /// </summary>
    public DateTime PurchaseDate { get; set; }

    /// <summary>
    /// The list of individual items purchased in this transaction.
    /// </summary>
    public List<PurchaseInvoiceItem> Items { get; set; } = new();

    /// <summary>
    /// The total price paid by the user after discounts (sum of PriceAtPurchase).
    /// </summary>
    public decimal TotalPrice => Items.Sum(i => i.PriceAtPurchase);

    /// <summary>
    /// The total original price before any discounts were applied.
    /// Null if no discount was involved.
    /// </summary>
    public decimal? OriginalTotal { get; set; }

    /// <summary>
    /// The percentage discount applied to the total, if any.
    /// </summary>
    public int? DiscountPercent { get; set; }   
}
