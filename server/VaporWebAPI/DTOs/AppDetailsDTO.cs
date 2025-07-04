namespace VaporWebAPI.DTOs;

/// <summary>
/// Data Transfer Object containing detailed information about an app, including media, pricing, and metadata.
/// </summary>
public class AppDetailsDTO
{
    /// <summary>
    /// Unique identifier of the application.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// The ID of the base application this app is related to (e.g., DLC, soundtrack).
    /// Null if it is a base game.
    /// </summary>
    public int? BaseAppId { get; set; }

    /// <summary>
    /// The name/title of the application.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// URL of the header image displayed on the app detail page.
    /// </summary>
    public string HeaderImage { get; set; }

    /// <summary>
    /// Price of the application, displayed as a formatted string (e.g., "0", "9.99€").
    /// </summary>
    public string Price { get; set; }

    /// <summary>
    /// Short description or summary of the application.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// Release date of the application (can be "Unknown").
    /// </summary>
    public string ReleaseDate { get; set; }

    /// <summary>
    /// List of genre names associated with the application.
    /// </summary>
    public List<string> Genres { get; set; }

    /// <summary>
    /// List of developers responsible for creating the application.
    /// </summary>
    public List<string> Developer { get; set; }

    /// <summary>
    /// List of publishers who released the application.
    /// </summary>
    public List<string> Publisher { get; set; }

    /// <summary>
    /// Dictionary mapping full screenshot URLs to their corresponding thumbnail URLs.
    /// </summary>
    public Dictionary<string, string> Screenshots { get; set; }

    /// <summary>
    /// Dictionary mapping full video URLs to their corresponding thumbnail URLs.
    /// </summary>
    public Dictionary<string, string> Videos { get; set; }


    /// <summary>
    /// Initializes default collections to avoid null references.
    /// </summary>
    public AppDetailsDTO()
    {
        Genres = new List<string>();
        Developer = new List<string>();
        Publisher = new List<string>();
        Screenshots = new Dictionary<string, string>();
        Videos = new Dictionary<string, string>();
    }
}
