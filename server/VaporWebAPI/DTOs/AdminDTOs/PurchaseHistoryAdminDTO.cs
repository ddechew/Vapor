namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object representing a user's purchase history entry for admin views.
/// </summary>
public class PurchaseHistoryAdminDTO
{
    /// <summary>
    /// Unique identifier of the library entry (purchase record).
    /// </summary>
    public int AppLibraryId { get; set; }

    /// <summary>
    /// Identifier of the purchased application.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Name of the purchased application.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// Identifier of the user who made the purchase (nullable if user deleted).
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// Display name of the user who made the purchase.
    /// </summary>
    public string UserDisplayName { get; set; }

    /// <summary>
    /// Date and time when the purchase was made.
    /// </summary>
    public DateTime PurchaseDate { get; set; }
}
