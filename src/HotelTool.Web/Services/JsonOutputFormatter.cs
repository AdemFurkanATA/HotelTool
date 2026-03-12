using System.Text.Json;
using HotelTool.Web.Models;

namespace HotelTool.Web.Services;

/// <summary>
/// Writes and reads hotel data in JSON format.
/// </summary>
public class JsonOutputFormatter : IOutputFormatter
{
    public string FormatName => "JSON";
    public string FileExtension => ".json";

    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    public void Write(IEnumerable<Hotel> hotels, string outputPath)
    {
        var json = JsonSerializer.Serialize(hotels, Options);
        File.WriteAllText(outputPath, json);
    }

    public List<Hotel> Read(string filePath)
    {
        if (!File.Exists(filePath)) return new List<Hotel>();

        var json = File.ReadAllText(filePath);
        return JsonSerializer.Deserialize<List<Hotel>>(json, Options) ?? new List<Hotel>();
    }
}
