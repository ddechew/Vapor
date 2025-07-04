namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object representing an application with administrative details.
/// </summary>
public class AppAdminDTO
{
    /// <summary>
    /// Unique identifier for the application.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// Name of the application.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Price of the application.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Description of the application (optional).
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Identifier of the application type.
    /// </summary>
    public int AppTypeId { get; set; }

    /// <summary>
    /// Name of the application type.
    /// </summary>
    public string AppTypeName { get; set; }
}
