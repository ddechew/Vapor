namespace VaporWebAPI.DTOs;

/// <summary>
/// Represents a summarized view of an application for display in the store.
/// </summary>
public class StoreAppDTO
{
    /// <summary>
    /// The unique identifier of the application.
    /// </summary>
    public int AppId { get; set; }

    /// <summary>
    /// The name of the application.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The URL to the application's header image.
    /// </summary>
    public string HeaderImage { get; set; }

    /// <summary>
    /// The display price of the application (e.g., "Free", "9.99€").
    /// </summary>
    public string Price { get; set; }

    /// <summary>
    /// The URL to the application's trailer video.
    /// </summary>
    public string Trailer { get; set; }

    /// <summary>
    /// A list of genre names associated with the application.
    /// </summary>
    public List<string> Genres { get; set; }

    /// <summary>
    /// The release date of the application (formatted string).
    /// </summary>
    public string ReleaseDate { get; set; }

    /// <summary>
    /// A short description of the application for preview.
    /// </summary>
    public string ShortDescription { get; set; } 
}
