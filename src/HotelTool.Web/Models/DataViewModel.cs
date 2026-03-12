namespace HotelTool.Web.Models;

/// <summary>
/// View model for the data visualization page with sorting and grouping support.
/// </summary>
public class DataViewModel
{
    public List<Hotel> Hotels { get; set; } = new();
    public string? SortBy { get; set; }
    public string? GroupBy { get; set; }
    public Dictionary<string, List<Hotel>>? GroupedHotels { get; set; }
    public bool HasData => Hotels.Any() || (GroupedHotels?.Any() ?? false);
    public string? SourceFile { get; set; }
}
