namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents a single item included in a user's purchase invoice,
/// typically used to generate email receipts or transaction summaries.
/// </summary>
public class PurchaseInvoiceItem
{
    /// <summary>
    /// The name of the purchased application.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The URL of the header image representing the app.
    /// </summary>
    public string HeaderImage { get; set; } = string.Empty;

    /// <summary>
    /// The price the user paid at the time of purchase,
    /// possibly discounted.
    /// </summary>
    public decimal PriceAtPurchase { get; set; }

    /// <summary>
    /// The original price of the app before any discount.
    /// Null if no discount was applied.
    /// </summary>
    public decimal? OriginalPrice { get; set; }
}
