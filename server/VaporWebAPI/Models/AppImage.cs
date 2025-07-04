namespace VaporWebAPI.Models;

/// <summary>
/// Represents an image associated with a specific application.
/// Can include headers, screenshots, thumbnails, etc.
/// </summary>
public partial class AppImage
{
    /// <summary>
    /// Unique identifier for the image.
    /// </summary>
    public int ImageId { get; set; }

    /// <summary>
    /// ID of the associated app.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// URL to the full-size image.
    /// </summary>
    public string ImageUrl { get; set; } = null!;

    /// <summary>
    /// Optional URL to a thumbnail version of the image.
    /// </summary>
    public string? ThumbnailUrl { get; set; }

    /// <summary>
    /// Type of image (e.g., "header", "screenshot", "icon").
    /// </summary>
    public string ImageType { get; set; } = null!;

    /// <summary>
    /// Timestamp of the last modification (required suffix 21180128).
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property to the associated App.
    /// </summary>
    public virtual App App { get; set; } = null!;
}
