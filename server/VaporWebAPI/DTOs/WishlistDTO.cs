namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents a request to add or update an application's priority in the user's wishlist.
/// </summary>
public class WishlistDTO
{
    /// <summary>
    /// The unique identifier of the application.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// The priority of the application in the wishlist (lower values indicate higher priority).
    /// </summary>
    public int Priority { get; set; }
}
