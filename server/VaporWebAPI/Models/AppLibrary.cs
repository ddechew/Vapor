namespace VaporWebAPI.Models;

/// <summary>
/// Represents a record of an app owned by a user, typically added after purchase or claiming a free app.
/// </summary>
public partial class AppLibrary
{
    /// <summary>
    /// Unique identifier for the library record.
    /// </summary>
    public int LibraryId { get; set; }

    /// <summary>
    /// ID of the user who owns the app.
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// ID of the app that is owned.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Date and time when the app was added to the library.
    /// </summary>
    public DateTime PurchaseDate { get; set; }

    /// <summary>
    /// Timestamp of the last modification (required suffix 21180128).
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the associated app.
    /// </summary>
    public virtual App App { get; set; } = null!;

    /// <summary>
    /// Navigation property to the owning user.
    /// </summary>
    public virtual User User { get; set; } = null!;
}
