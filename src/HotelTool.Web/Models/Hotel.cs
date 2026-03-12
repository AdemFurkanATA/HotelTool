namespace HotelTool.Web.Models;

/// <summary>
/// Represents a hotel entity parsed from the CSV file.
/// </summary>
public class Hotel
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int Stars { get; set; }
    public string Contact { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
}
