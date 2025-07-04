namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents a request to merge a guest user's cart into a logged-in user's cart.
/// </summary>
public class MergeCartRequest
{
    /// <summary>
    /// The ID of the logged-in user.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// A list of app IDs from the guest user's cart that should be added to the user's cart.
    /// </summary>
    public List<int> GuestAppIds { get; set; }
}
