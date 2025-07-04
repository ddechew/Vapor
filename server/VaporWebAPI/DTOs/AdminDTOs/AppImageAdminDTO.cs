namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object representing an application's image details for administrative purposes.
/// </summary>
public class AppImageAdminDTO
{
    /// <summary>
    /// Unique identifier of the image.
    /// </summary>
    public int ImageId { get; set; }

    /// <summary>
    /// Identifier of the associated application.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Name of the associated application.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// URL of the image.
    /// </summary>
    public string ImageUrl { get; set; }

    /// <summary>
    /// Timestamp when the image was added or last modified.
    /// </summary>
    public DateTime AddedAt { get; set; }
}
