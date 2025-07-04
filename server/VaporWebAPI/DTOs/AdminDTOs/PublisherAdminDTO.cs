namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object representing a publisher for admin views.
/// </summary>
public class PublisherAdminDTO
{
    /// <summary>
    /// Unique identifier of the publisher.
    /// </summary>
    public int PublisherId { get; set; }

    /// <summary>
    /// Name of the publisher.
    /// </summary>
    public string PublisherName { get; set; }
}
