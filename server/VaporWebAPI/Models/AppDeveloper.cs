namespace VaporWebAPI.Models;

/// <summary>
/// Join entity representing the many-to-many relationship between Apps and Developers.
/// </summary>
public partial class AppDeveloper
{
    /// <summary>
    /// ID of the associated app.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// ID of the associated developer.
    /// </summary>
    public int DeveloperId { get; set; }

    /// <summary>
    /// Timestamp of the last modification (required suffix 21180128).
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property for the associated App.
    /// </summary>
    public virtual App App { get; set; } = null!;

    /// <summary>
    /// Navigation property for the associated Developer.
    /// </summary>
    public virtual Developer Developer { get; set; } = null!;
}
