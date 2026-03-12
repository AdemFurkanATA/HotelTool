using System.Xml.Serialization;
using HotelTool.Web.Models;

namespace HotelTool.Web.Services;

/// <summary>
/// Writes and reads hotel data in XML format.
/// </summary>
public class XmlOutputFormatter : IOutputFormatter
{
    public string FormatName => "XML";
    public string FileExtension => ".xml";

    public void Write(IEnumerable<Hotel> hotels, string outputPath)
    {
        var hotelList = hotels.ToList();
        var serializer = new XmlSerializer(typeof(List<Hotel>), new XmlRootAttribute("Hotels"));

        using var writer = new StreamWriter(outputPath, false, System.Text.Encoding.UTF8);
        serializer.Serialize(writer, hotelList);
    }

    public List<Hotel> Read(string filePath)
    {
        if (!File.Exists(filePath)) return new List<Hotel>();

        var serializer = new XmlSerializer(typeof(List<Hotel>), new XmlRootAttribute("Hotels"));

        using var reader = new StreamReader(filePath);
        return serializer.Deserialize(reader) as List<Hotel> ?? new List<Hotel>();
    }
}
