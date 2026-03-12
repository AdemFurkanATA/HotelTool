using HotelTool.Web.Models;

namespace HotelTool.Web.Services;

/// <summary>
/// Validates hotel data according to the defined rules:
/// - Hotel name may only contain valid UTF-8 characters.
/// - Hotel URL must be valid (well-formed URI with HTTP/HTTPS scheme).
/// - Hotel rating must be between 0 and 5 (inclusive), no negatives.
/// </summary>
public class HotelValidator
{
    /// <summary>
    /// Validates a list of hotels and returns the validation result.
    /// </summary>
    public HotelValidationResult Validate(List<Hotel> hotels)
    {
        var result = new HotelValidationResult();

        for (int i = 0; i < hotels.Count; i++)
        {
            var errors = ValidateHotel(hotels[i]);

            if (errors.Count == 0)
            {
                result.ValidHotels.Add(hotels[i]);
            }
            else
            {
                result.InvalidEntries.Add(new InvalidHotelEntry
                {
                    LineNumber = i + 2, // +2 because line 1 is header, index is 0-based
                    RawData = $"{hotels[i].Name}, {hotels[i].Address}, {hotels[i].Stars}, {hotels[i].Uri}",
                    Errors = errors
                });
            }
        }

        return result;
    }

    /// <summary>
    /// Validates a single hotel and returns a list of error messages.
    /// An empty list means the hotel is valid.
    /// </summary>
    public List<string> ValidateHotel(Hotel hotel)
    {
        var errors = new List<string>();

        // Rule 1: Hotel name must contain only valid UTF-8 characters
        if (!IsValidUtf8Name(hotel.Name))
        {
            errors.Add("Hotel name contains invalid characters.");
        }

        if (string.IsNullOrWhiteSpace(hotel.Name))
        {
            errors.Add("Hotel name cannot be empty.");
        }

        // Rule 2: Hotel URL must be valid
        if (!IsValidUrl(hotel.Uri))
        {
            errors.Add($"Invalid URL: '{hotel.Uri}'");
        }

        // Rule 3: Hotel rating must be between 0 and 5
        if (!IsValidRating(hotel.Stars))
        {
            errors.Add($"Invalid rating: {hotel.Stars}. Rating must be between 0 and 5.");
        }

        return errors;
    }

    /// <summary>
    /// Checks if the hotel name contains only valid UTF-8 characters.
    /// In C#, strings are UTF-16 internally. We check for replacement characters
    /// which indicate invalid encoding, and for control characters.
    /// </summary>
    public static bool IsValidUtf8Name(string name)
    {
        if (string.IsNullOrEmpty(name)) return false;

        foreach (char c in name)
        {
            // Check for Unicode replacement character (indicates encoding error)
            if (c == '\uFFFD') return false;

            // Check for control characters (except common whitespace)
            if (char.IsControl(c) && c != '\t' && c != '\n' && c != '\r') return false;
        }

        return true;
    }

    /// <summary>
    /// Validates a URL. A valid URL must:
    /// - Be a well-formed absolute URI
    /// - Use HTTP or HTTPS scheme
    /// - Have a non-empty host
    /// </summary>
    public static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return false;

        if (!System.Uri.TryCreate(url, UriKind.Absolute, out var uri)) return false;

        // Must be HTTP or HTTPS
        if (uri.Scheme != "http" && uri.Scheme != "https") return false;

        // Must have a host
        if (string.IsNullOrWhiteSpace(uri.Host)) return false;

        // Host should have at least one dot (e.g., example.com)
        // This filters out URLs like http://localhost which are not real hotel URLs
        if (!uri.Host.Contains('.')) return false;

        return true;
    }

    /// <summary>
    /// Validates the hotel rating. Must be between 0 and 5 inclusive.
    /// </summary>
    public static bool IsValidRating(int stars)
    {
        return stars >= 0 && stars <= 5;
    }
}
