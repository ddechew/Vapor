namespace VaporWebAPI.Models;

/// <summary>
/// Represents a game developer entity.
/// </summary>
public partial class Developer
{
    /// <summary>
    /// Primary key of the developer.
    /// </summary>
    public int DeveloperId { get; set; }

    /// <summary>
    /// Name of the developer or development studio.
    /// </summary>
    public string DeveloperName { get; set; } = null!;

    /// <summary>
    /// Timestamp of the last modification (suffix required: 21180128).
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property linking to all apps developed by this developer.
    /// </summary>
    public virtual ICollection<AppDeveloper> AppDevelopers { get; set; } = new List<AppDeveloper>();
}
