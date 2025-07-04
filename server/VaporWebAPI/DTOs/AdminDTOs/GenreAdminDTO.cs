namespace VaporWebAPI.DTOs.AdminDTOs;

/// <summary>
/// Data Transfer Object representing a genre in the admin panel.
/// </summary>
public class GenreAdminDTO
{
    /// <summary>
    /// Unique identifier of the genre.
    /// </summary>
    public int GenreId { get; set; }

    /// <summary>
    /// Name of the genre.
    /// </summary>
    public string Name { get; set; }
}
