namespace VaporWebAPI.Models;

/// <summary>
/// Represents the many-to-many relationship between apps and publishers.
/// </summary>
public partial class AppPublisher
{
    /// <summary>
    /// ID of the app associated with the publisher.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// ID of the publisher associated with the app.
    /// </summary>
    public int PublisherId { get; set; }

    /// <summary>
    /// Timestamp of the last modification (required suffix 21180128).
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the associated app.
    /// </summary>
    public virtual App App { get; set; } = null!;

    /// <summary>
    /// Navigation property to the associated publisher.
    /// </summary>
    public virtual Publisher Publisher { get; set; } = null!;
}
