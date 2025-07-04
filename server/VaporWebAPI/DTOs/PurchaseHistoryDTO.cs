namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents a record of a user's purchase history, including app details, payment method, and wallet data.
/// </summary>
public class PurchaseHistoryDTO
{
    /// <summary>
    /// The ID of the purchased app. Null if the record is a wallet top-up.
    /// </summary>
    public int? AppId { get; set; }

    /// <summary>
    /// The name of the purchased app, or a label like "Wallet Top-Up" for non-app purchases.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// The URL of the app's header image, if available.
    /// </summary>
    public string? AppHeaderImage { get; set; }

    /// <summary>
    /// The method used to complete the purchase (e.g., "Wallet", "Stripe", "Free").
    /// </summary>
    public string PaymentMethod { get; set; }

    /// <summary>
    /// The price of the app at the time of purchase.
    /// </summary>
    public decimal PriceAtPurchase { get; set; }

    /// <summary>
    /// The change applied to the user's wallet as a result of the purchase.
    /// Negative for deductions, positive for top-ups. Null for free purchases.
    /// </summary>
    public decimal? WalletChange { get; set; }

    /// <summary>
    /// The resulting wallet balance after the transaction. Null for free purchases.
    /// </summary>
    public decimal? WalletBalanceAfter { get; set; }

    /// <summary>
    /// The timestamp when the purchase occurred.
    /// </summary>
    public DateTime PurchaseDate { get; set; }
}
