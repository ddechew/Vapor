namespace VaporWebAPI.DTOs;

/// <summary>
/// Data Transfer Object representing a single item in the user's cart.
/// </summary>
public class CartItemDTO
{
    /// <summary>
    /// Unique identifier of the application.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Name of the application.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Price of the application as a formatted string (e.g., "9.99€" or "Free").
    /// </summary>
    public string Price { get; set; }

    /// <summary>
    /// URL to the header image representing the application.
    /// </summary>
    public string HeaderImage { get; set; }

    /// <summary>
    /// Optional ID of the base application, if this app is related content (e.g., DLC or expansion).
    /// </summary>
    public int? BaseAppId { get; set; }
}
