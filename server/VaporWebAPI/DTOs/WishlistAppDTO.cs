namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents an application entry in a user's wishlist.
/// </summary>
public class WishlistAppDTO
{
    /// <summary>
    /// The unique identifier of the application.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// The name of the application.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The URL to the header image of the application.
    /// </summary>
    public string HeaderImage { get; set; }

    /// <summary>
    /// The price of the application displayed as a formatted string (e.g., "Free", "9.99€").
    /// </summary>
    public string Price { get; set; }

    /// <summary>
    /// The priority of the application in the wishlist (lower values indicate higher priority).
    /// </summary>
    public int Priority { get; set; }
}
