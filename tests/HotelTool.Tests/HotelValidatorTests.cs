using HotelTool.Web.Models;
using HotelTool.Web.Services;

namespace HotelTool.Tests;

public class HotelValidatorTests
{
    private readonly HotelValidator _validator = new();

    [Fact]
    public void ValidHotel_ShouldPassValidation()
    {
        var hotel = new Hotel
        {
            Name = "Grand Hotel",
            Address = "123 Main St",
            Stars = 5,
            Contact = "John Doe",
            Phone = "+1-555-0100",
            Uri = "http://www.grandhotel.com"
        };

        var errors = _validator.ValidateHotel(hotel);
        Assert.Empty(errors);
    }

    [Fact]
    public void HotelWithNegativeRating_ShouldFail()
    {
        var hotel = new Hotel
        {
            Name = "Bad Hotel",
            Address = "456 Elm St",
            Stars = -1,
            Contact = "Jane Doe",
            Phone = "+1-555-0200",
            Uri = "http://www.badhotel.com"
        };

        var errors = _validator.ValidateHotel(hotel);
        Assert.Contains(errors, e => e.Contains("rating") || e.Contains("Rating"));
    }

    [Fact]
    public void HotelWithRatingAbove5_ShouldFail()
    {
        var hotel = new Hotel
        {
            Name = "Super Hotel",
            Address = "789 Oak Ave",
            Stars = 6,
            Contact = "Bob Builder",
            Phone = "+1-555-0300",
            Uri = "http://www.superhotel.com"
        };

        var errors = _validator.ValidateHotel(hotel);
        Assert.Contains(errors, e => e.Contains("rating") || e.Contains("Rating"));
    }

    [Fact]
    public void HotelWithZeroRating_ShouldPass()
    {
        var hotel = new Hotel
        {
            Name = "Zero Star Hotel",
            Address = "321 Pine St",
            Stars = 0,
            Contact = "Zero Test",
            Phone = "+1-555-0400",
            Uri = "http://www.zerostarhotel.com"
        };

        var errors = _validator.ValidateHotel(hotel);
        Assert.Empty(errors);
    }

    [Fact]
    public void HotelWithInvalidUrl_ShouldFail()
    {
        var hotel = new Hotel
        {
            Name = "No URL Hotel",
            Address = "111 Elm St",
            Stars = 3,
            Contact = "Tester",
            Phone = "+1-555-0500",
            Uri = "not-a-valid-url"
        };

        var errors = _validator.ValidateHotel(hotel);
        Assert.Contains(errors, e => e.Contains("URL") || e.Contains("Url"));
    }

    [Fact]
    public void HotelWithEmptyUrl_ShouldFail()
    {
        var hotel = new Hotel
        {
            Name = "Empty URL Hotel",
            Address = "222 Elm St",
            Stars = 2,
            Contact = "Tester",
            Phone = "+1-555-0600",
            Uri = ""
        };

        var errors = _validator.ValidateHotel(hotel);
        Assert.Contains(errors, e => e.Contains("URL") || e.Contains("Url"));
    }

    [Fact]
    public void HotelWithFtpUrl_ShouldFail()
    {
        var hotel = new Hotel
        {
            Name = "FTP Hotel",
            Address = "333 Elm St",
            Stars = 4,
            Contact = "Tester",
            Phone = "+1-555-0700",
            Uri = "ftp://ftp.hotel.com/files"
        };

        var errors = _validator.ValidateHotel(hotel);
        Assert.Contains(errors, e => e.Contains("URL") || e.Contains("Url"));
    }

    [Fact]
    public void HotelWithEmptyName_ShouldFail()
    {
        var hotel = new Hotel
        {
            Name = "",
            Address = "444 Elm St",
            Stars = 3,
            Contact = "Tester",
            Phone = "+1-555-0800",
            Uri = "http://www.hotel.com"
        };

        var errors = _validator.ValidateHotel(hotel);
        Assert.NotEmpty(errors);
    }

    [Fact]
    public void HotelWithUnicodeCharsInName_ShouldPass()
    {
        var hotel = new Hotel
        {
            Name = "Hôtel Château Müller",
            Address = "Paris, France",
            Stars = 5,
            Contact = "François",
            Phone = "+33-1-555-0900",
            Uri = "http://www.chateau-muller.fr"
        };

        var errors = _validator.ValidateHotel(hotel);
        Assert.Empty(errors);
    }

    [Fact]
    public void ValidateList_ShouldSeparateValidAndInvalid()
    {
        var hotels = new List<Hotel>
        {
            new() { Name = "Good Hotel", Stars = 3, Uri = "http://www.good.com" },
            new() { Name = "Bad Hotel", Stars = -1, Uri = "http://www.bad.com" },
            new() { Name = "Great Hotel", Stars = 5, Uri = "http://www.great.com" }
        };

        var result = _validator.Validate(hotels);
        Assert.Equal(2, result.ValidHotels.Count);
        Assert.Single(result.InvalidEntries);
    }

    [Theory]
    [InlineData("http://www.hotel.com", true)]
    [InlineData("https://hotel.com/search", true)]
    [InlineData("http://hotel.com/path?q=1", true)]
    [InlineData("ftp://hotel.com", false)]
    [InlineData("not-a-url", false)]
    [InlineData("", false)]
    [InlineData("http://localhost", false)]
    public void IsValidUrl_ShouldValidateCorrectly(string url, bool expected)
    {
        Assert.Equal(expected, HotelValidator.IsValidUrl(url));
    }

    [Theory]
    [InlineData(0, true)]
    [InlineData(1, true)]
    [InlineData(5, true)]
    [InlineData(-1, false)]
    [InlineData(6, false)]
    [InlineData(100, false)]
    public void IsValidRating_ShouldValidateCorrectly(int rating, bool expected)
    {
        Assert.Equal(expected, HotelValidator.IsValidRating(rating));
    }
}
