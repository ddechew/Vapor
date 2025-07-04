namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object representing a user's game library entry for administrative views.
/// </summary>
public class UserLibraryAdminDTO
{
    /// <summary>
    /// Unique identifier of the library entry.
    /// </summary>
    public int LibraryId { get; set; }

    /// <summary>
    /// Identifier of the user who owns the library entry.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Display name of the user; nullable if user is deleted.
    /// </summary>
    public string? UserDisplayName { get; set; }

    /// <summary>
    /// Identifier of the app owned by the user.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Name of the owned app.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// Date of purchase or acquisition of the app.
    /// </summary>
    public DateTime PurchaseDate { get; set; }
}
