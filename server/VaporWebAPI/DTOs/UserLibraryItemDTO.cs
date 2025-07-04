namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents an item in a user's game library, including related applications.
/// </summary>
public class UserLibraryItemDTO
{
    /// <summary>
    /// The unique identifier of the application.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// The name of the application.
    /// </summary>
    public string AppName { get; set; }

    /// <summary>
    /// The price of the application, if applicable.
    /// </summary>
    public decimal? Price { get; set; }

    /// <summary>
    /// The type ID of the application (e.g., game, DLC, etc.).
    /// </summary>
    public int AppTypeId { get; set; }

    /// <summary>
    /// The URL to the header image of the application.
    /// </summary>
    public string HeaderImage { get; set; }

    /// <summary>
    /// The date the application was purchased or added to the library.
    /// </summary>
    public DateTime PurchaseDate { get; set; }

    /// <summary>
    /// A list of related applications (e.g., DLCs, expansions).
    /// </summary>
    public List<RelatedAppDTO> RelatedApps { get; set; } = new();
}
