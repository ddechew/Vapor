namespace VaporWebAPI.Models;

/// <summary>
/// Represents a video (e.g., trailer or gameplay preview) associated with a specific app.
/// </summary>
public partial class AppVideo
{
    /// <summary>
    /// Unique identifier for the video entry.
    /// </summary>
    public int VideoId { get; set; }

    /// <summary>
    /// Foreign key referencing the related app.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// URL to the embedded video (e.g., YouTube embed link).
    /// </summary>
    public string VideoUrl { get; set; } = null!;

    /// <summary>
    /// URL to the thumbnail image representing the video.
    /// </summary>
    public string ThumbnailUrl { get; set; } = null!;

    /// <summary>
    /// Timestamp of the last modification (suffix required: 21180128).
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the associated app.
    /// </summary>
    public virtual App App { get; set; } = null!;
}
