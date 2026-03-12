namespace HotelTool.Web.Models;

/// <summary>
/// View model for displaying upload results to the user.
/// </summary>
public class UploadViewModel
{
    public bool HasResult { get; set; }
    public int ValidCount { get; set; }
    public int InvalidCount { get; set; }
    public List<string> OutputFiles { get; set; } = new();
    public List<InvalidHotelEntry> InvalidEntries { get; set; } = new();
    public string? ErrorMessage { get; set; }
}
