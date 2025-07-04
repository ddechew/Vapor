namespace VaporWebAPI.Models;

/// <summary>
/// Represents a category/type of application (e.g., Game, DLC, Demo, Soundtrack).
/// </summary>
public partial class AppType
{
    /// <summary>
    /// Unique identifier for the application type.
    /// </summary>
    public int AppTypeId { get; set; }

    /// <summary>
    /// Descriptive name of the application type.
    /// </summary>
    public string TypeName { get; set; } = null!;

    /// <summary>
    /// Timestamp of the last modification (suffix required: 21180128).
    /// </summary>
    public DateTime LastModification21180128 { get; set; }

    /// <summary>
    /// Navigation property representing all apps associated with this type.
    /// </summary>
    public virtual ICollection<App> Apps { get; set; } = new List<App>();
}
