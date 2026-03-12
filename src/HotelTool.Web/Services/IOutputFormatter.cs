using HotelTool.Web.Models;

namespace HotelTool.Web.Services;

/// <summary>
/// Interface for output formatters. Implement this interface to add
/// support for a new output format. Register the implementation in DI
/// and it will be automatically available.
/// </summary>
public interface IOutputFormatter
{
    /// <summary>
    /// Display name of the format (e.g., "JSON", "XML").
    /// </summary>
    string FormatName { get; }

    /// <summary>
    /// File extension including the dot (e.g., ".json", ".xml").
    /// </summary>
    string FileExtension { get; }

    /// <summary>
    /// Writes the hotel data to the specified output path.
    /// </summary>
    void Write(IEnumerable<Hotel> hotels, string outputPath);

    /// <summary>
    /// Reads hotel data from the specified file path.
    /// </summary>
    List<Hotel> Read(string filePath);
}
