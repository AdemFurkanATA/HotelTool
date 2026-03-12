namespace HotelTool.Web.Models;

/// <summary>
/// Holds the result of CSV processing: valid hotels, invalid entries, and output file paths.
/// </summary>
public class HotelValidationResult
{
    public List<Hotel> ValidHotels { get; set; } = new();
    public List<InvalidHotelEntry> InvalidEntries { get; set; } = new();
    public List<string> OutputFiles { get; set; } = new();
}

/// <summary>
/// Represents an invalid hotel entry with its validation errors.
/// </summary>
public class InvalidHotelEntry
{
    public int LineNumber { get; set; }
    public string RawData { get; set; } = string.Empty;
    public List<string> Errors { get; set; } = new();
}
