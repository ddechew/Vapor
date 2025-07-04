namespace VaporWebAPI.DTOs;

/// <summary>
/// Data Transfer Object used to add an item to a user's cart.
/// </summary>
public class CartRequest
{
    /// <summary>
    /// The unique identifier of the user performing the action.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// The unique identifier of the application being added to the cart.
    /// </summary>
    public int AppId { get; set; }
}
