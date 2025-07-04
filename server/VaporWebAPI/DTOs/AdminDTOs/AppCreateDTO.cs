namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object for creating a new application.
/// </summary>
public class AppCreateDTO
{
    /// <summary>
    /// Name of the application.
    /// </summary>
    public string AppName { get; set; } = null!;

    /// <summary>
    /// Description of the application.
    /// </summary>
    public string Description { get; set; } = null!;

    /// <summary>
    /// Price of the application.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// Identifier of the application type.
    /// </summary>
    public int AppTypeId { get; set; }

    /// <summary>
    /// Optional base application identifier for related apps.
    /// </summary>
    public int? BaseAppId { get; set; }  
}
