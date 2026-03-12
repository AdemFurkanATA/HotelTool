using HotelTool.Web.Models;
using HotelTool.Web.Services;

namespace HotelTool.Tests;

public class OutputFormatterTests : IDisposable
{
    private readonly string _tempDir;

    public OutputFormatterTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "HotelToolTests_" + Guid.NewGuid().ToString("N")[..8]);
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
            Directory.Delete(_tempDir, true);
    }

    private static List<Hotel> GetTestHotels() => new()
    {
        new Hotel { Name = "Hotel A", Address = "123 Street", Stars = 5, Contact = "John", Phone = "+1-555", Uri = "http://a.com" },
        new Hotel { Name = "Hotel B", Address = "456 Avenue", Stars = 3, Contact = "Jane", Phone = "+1-666", Uri = "http://b.com" },
        new Hotel { Name = "Hötel Ç", Address = "Münih, Germany", Stars = 4, Contact = "Hans", Phone = "+49-123", Uri = "http://c.de" }
    };

    [Fact]
    public void JsonFormatter_ShouldWriteAndReadCorrectly()
    {
        var formatter = new JsonOutputFormatter();
        var outputPath = Path.Combine(_tempDir, "test.json");
        var hotels = GetTestHotels();

        formatter.Write(hotels, outputPath);

        Assert.True(File.Exists(outputPath));

        var readBack = formatter.Read(outputPath);
        Assert.Equal(3, readBack.Count);
        Assert.Equal("Hotel A", readBack[0].Name);
        Assert.Equal(5, readBack[0].Stars);
        Assert.Equal("Hötel Ç", readBack[2].Name);
    }

    [Fact]
    public void XmlFormatter_ShouldWriteAndReadCorrectly()
    {
        var formatter = new XmlOutputFormatter();
        var outputPath = Path.Combine(_tempDir, "test.xml");
        var hotels = GetTestHotels();

        formatter.Write(hotels, outputPath);

        Assert.True(File.Exists(outputPath));

        var readBack = formatter.Read(outputPath);
        Assert.Equal(3, readBack.Count);
        Assert.Equal("Hotel A", readBack[0].Name);
        Assert.Equal("Hötel Ç", readBack[2].Name);
    }

    [Fact]
    public void JsonFormatter_ShouldHaveCorrectMetadata()
    {
        var formatter = new JsonOutputFormatter();
        Assert.Equal("JSON", formatter.FormatName);
        Assert.Equal(".json", formatter.FileExtension);
    }

    [Fact]
    public void XmlFormatter_ShouldHaveCorrectMetadata()
    {
        var formatter = new XmlOutputFormatter();
        Assert.Equal("XML", formatter.FormatName);
        Assert.Equal(".xml", formatter.FileExtension);
    }

    [Fact]
    public void JsonFormatter_EmptyList_ShouldWriteEmptyArray()
    {
        var formatter = new JsonOutputFormatter();
        var outputPath = Path.Combine(_tempDir, "empty.json");

        formatter.Write(new List<Hotel>(), outputPath);

        var content = File.ReadAllText(outputPath);
        Assert.Equal("[]", content.Trim());
    }

    [Fact]
    public void OutputFormatterFactory_ShouldResolveFormatters()
    {
        var formatters = new IOutputFormatter[]
        {
            new JsonOutputFormatter(),
            new XmlOutputFormatter()
        };

        var factory = new OutputFormatterFactory(formatters);

        Assert.NotNull(factory.GetFormatter("JSON"));
        Assert.NotNull(factory.GetFormatter("json"));
        Assert.NotNull(factory.GetFormatter("XML"));
        Assert.Null(factory.GetFormatter("YAML"));
        Assert.Equal(2, factory.GetAllFormatters().Count);
    }
}
