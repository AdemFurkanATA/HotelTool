using HotelTool.Web.Services;

namespace HotelTool.Tests;

public class CsvReaderServiceTests
{
    private readonly CsvReaderService _reader = new();

    [Fact]
    public void ReadFromStream_ShouldParseValidCsv()
    {
        var csv = "name,address,stars,contact,phone,uri\nHotel One,\"123 Main St, City\",5,John Doe,+1-555-0100,http://www.hotel.com\n";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

        var hotels = _reader.ReadFromStream(stream);

        Assert.Single(hotels);
        Assert.Equal("Hotel One", hotels[0].Name);
        Assert.Equal("123 Main St, City", hotels[0].Address);
        Assert.Equal(5, hotels[0].Stars);
        Assert.Equal("http://www.hotel.com", hotels[0].Uri);
    }

    [Fact]
    public void ReadFromStream_ShouldHandleCarriageReturnSeparator()
    {
        var csv = "name,address,stars,contact,phone,uri\rHotel A,Addr A,3,Contact A,Phone A,http://a.com\rHotel B,Addr B,4,Contact B,Phone B,http://b.com\r";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

        var hotels = _reader.ReadFromStream(stream);

        Assert.Equal(2, hotels.Count);
        Assert.Equal("Hotel A", hotels[0].Name);
        Assert.Equal("Hotel B", hotels[1].Name);
    }

    [Fact]
    public void ReadFromStream_ShouldSkipEmptyLines()
    {
        var csv = "name,address,stars,contact,phone,uri\n\nHotel X,Addr X,2,Contact X,Phone X,http://x.com\n\n";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

        var hotels = _reader.ReadFromStream(stream);

        Assert.Single(hotels);
    }

    [Fact]
    public void ReadFromStream_ShouldHandleQuotedFieldsWithCommas()
    {
        var csv = "name,address,stars,contact,phone,uri\nHotel Z,\"123 Street, Suite 456, City, ST 12345\",4,Contact Z,+1-555,http://z.com\n";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

        var hotels = _reader.ReadFromStream(stream);

        Assert.Single(hotels);
        Assert.Equal("123 Street, Suite 456, City, ST 12345", hotels[0].Address);
    }

    [Fact]
    public void ParseCsvLine_ShouldHandleSimpleLine()
    {
        var fields = CsvReaderService.ParseCsvLine("a,b,c,d,e,f");
        Assert.Equal(6, fields.Count);
        Assert.Equal("a", fields[0]);
        Assert.Equal("f", fields[5]);
    }

    [Fact]
    public void ParseCsvLine_ShouldHandleQuotedCommas()
    {
        var fields = CsvReaderService.ParseCsvLine("name,\"addr, city\",3,contact,phone,uri");
        Assert.Equal(6, fields.Count);
        Assert.Equal("addr, city", fields[1]);
    }

    [Fact]
    public void ReadFromStream_EmptyCsv_ShouldReturnEmptyList()
    {
        var csv = "";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

        var hotels = _reader.ReadFromStream(stream);

        Assert.Empty(hotels);
    }

    [Fact]
    public void ReadFromStream_HeaderOnly_ShouldReturnEmptyList()
    {
        var csv = "name,address,stars,contact,phone,uri\n";
        using var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(csv));

        var hotels = _reader.ReadFromStream(stream);

        Assert.Empty(hotels);
    }
}
