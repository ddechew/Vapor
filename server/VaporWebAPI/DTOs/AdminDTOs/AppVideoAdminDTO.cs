namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object representing a video associated with an application in the admin panel.
/// </summary>
public class AppVideoAdminDTO
{
    /// <summary>
    /// Unique identifier of the video.
    /// </summary>
    public int VideoId { get; set; }

    /// <summary>
    /// Identifier of the associated application.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Name of the associated application.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// URL of the video.
    /// </summary>
    public string VideoUrl { get; set; }

    /// <summary>
    /// Date and time when the video was added or last modified.
    /// </summary>
    public DateTime AddedAt { get; set; }
}
