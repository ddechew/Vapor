namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents a related application (e.g., DLC, soundtrack, etc.) for a base game.
/// </summary>
public class RelatedAppDTO
{
    /// <summary>
    /// The unique identifier of the related application.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// The name/title of the related application.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// The price of the related application. Null if not specified.
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// Indicates whether the user owns this related application.
    /// </summary>
    public bool IsOwned { get; set; }

    /// <summary>
    /// The type identifier of the application (e.g., base game, DLC, demo).
    /// </summary>
    public int AppTypeId { get; set; }

    /// <summary>
    /// The URL of the header image associated with the related application.
    /// </summary>
    public string HeaderImage { get; set; } 
}
