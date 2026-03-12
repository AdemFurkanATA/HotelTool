using HotelTool.Web.Models;

namespace HotelTool.Web.Services;

/// <summary>
/// Reads hotel data from a CSV file.
/// The first row is expected to be a header with field names.
/// </summary>
public class CsvReaderService
{
    /// <summary>
    /// Parses a CSV file and returns a list of Hotel objects.
    /// Handles quoted fields containing commas.
    /// </summary>
    public List<Hotel> ReadFromFile(string filePath)
    {
        var hotels = new List<Hotel>();
        var content = File.ReadAllText(filePath);

        // The CSV might use \r as line separator (as seen in hotels.csv)
        var lines = SplitLines(content);

        if (lines.Count == 0) return hotels;

        // Skip header row
        for (int i = 1; i < lines.Count; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var fields = ParseCsvLine(line);
            if (fields.Count < 6) continue;

            hotels.Add(new Hotel
            {
                Name = fields[0].Trim(),
                Address = fields[1].Trim(),
                Stars = int.TryParse(fields[2].Trim(), out var stars) ? stars : -1,
                Contact = fields[3].Trim(),
                Phone = fields[4].Trim(),
                Uri = fields[5].Trim()
            });
        }

        return hotels;
    }

    /// <summary>
    /// Parses a CSV file from a Stream and returns a list of Hotel objects.
    /// </summary>
    public List<Hotel> ReadFromStream(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var content = reader.ReadToEnd();

        var lines = SplitLines(content);
        var hotels = new List<Hotel>();

        if (lines.Count == 0) return hotels;

        for (int i = 1; i < lines.Count; i++)
        {
            var line = lines[i].Trim();
            if (string.IsNullOrWhiteSpace(line)) continue;

            var fields = ParseCsvLine(line);
            if (fields.Count < 6) continue;

            hotels.Add(new Hotel
            {
                Name = fields[0].Trim(),
                Address = fields[1].Trim(),
                Stars = int.TryParse(fields[2].Trim(), out var stars) ? stars : -1,
                Contact = fields[3].Trim(),
                Phone = fields[4].Trim(),
                Uri = fields[5].Trim()
            });
        }

        return hotels;
    }

    /// <summary>
    /// Splits content into lines, handling both \r\n, \n, and \r line endings.
    /// </summary>
    private static List<string> SplitLines(string content)
    {
        // Replace \r\n with \n first, then split by \r or \n
        content = content.Replace("\r\n", "\n").Replace("\r", "\n");
        return content.Split('\n', StringSplitOptions.None).ToList();
    }

    /// <summary>
    /// Parses a single CSV line, respecting quoted fields that may contain commas.
    /// </summary>
    public static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    // Escaped quote
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        fields.Add(current.ToString());
        return fields;
    }
}
