using Microsoft.AspNetCore.Mvc;
using HotelTool.Web.Models;
using HotelTool.Web.Services;

namespace HotelTool.Web.Controllers;

/// <summary>
/// Controller for hotel CSV upload, processing, and data visualization.
/// </summary>
public class HotelController : Controller
{
    private readonly CsvReaderService _csvReader;
    private readonly HotelValidator _validator;
    private readonly OutputFormatterFactory _formatterFactory;
    private readonly IWebHostEnvironment _environment;

    public HotelController(
        CsvReaderService csvReader,
        HotelValidator validator,
        OutputFormatterFactory formatterFactory,
        IWebHostEnvironment environment)
    {
        _csvReader = csvReader;
        _validator = validator;
        _formatterFactory = formatterFactory;
        _environment = environment;
    }

    /// <summary>
    /// GET / — Upload page.
    /// </summary>
    [HttpGet]
    public IActionResult Upload()
    {
        return View(new UploadViewModel());
    }

    /// <summary>
    /// POST /Hotel/Upload — Process uploaded CSV file.
    /// </summary>
    [HttpPost]
    public IActionResult Upload(IFormFile? csvFile)
    {
        var viewModel = new UploadViewModel();

        if (csvFile == null || csvFile.Length == 0)
        {
            viewModel.ErrorMessage = "Please select a CSV file to upload.";
            return View(viewModel);
        }

        if (!csvFile.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            viewModel.ErrorMessage = "Only CSV files are accepted.";
            return View(viewModel);
        }

        try
        {
            // 1. Read CSV data
            var hotels = _csvReader.ReadFromStream(csvFile.OpenReadStream());

            // 2. Validate
            var result = _validator.Validate(hotels);

            // 3. Determine output directory (same as input — we use Data folder)
            var outputDir = Path.Combine(_environment.ContentRootPath, "Data");
            Directory.CreateDirectory(outputDir);

            // 4. Write in all registered formats
            var outputFiles = new List<string>();
            foreach (var formatter in _formatterFactory.GetAllFormatters())
            {
                var outputPath = Path.Combine(outputDir, $"hotels{formatter.FileExtension}");
                formatter.Write(result.ValidHotels, outputPath);
                outputFiles.Add($"hotels{formatter.FileExtension}");
            }

            // 5. Also save the uploaded CSV
            var csvPath = Path.Combine(outputDir, "hotels.csv");
            using (var stream = new FileStream(csvPath, FileMode.Create))
            {
                csvFile.OpenReadStream().CopyTo(stream);
            }

            viewModel.HasResult = true;
            viewModel.ValidCount = result.ValidHotels.Count;
            viewModel.InvalidCount = result.InvalidEntries.Count;
            viewModel.OutputFiles = outputFiles;
            viewModel.InvalidEntries = result.InvalidEntries;
        }
        catch (Exception ex)
        {
            viewModel.ErrorMessage = $"An error occurred while processing the file: {ex.Message}";
        }

        return View(viewModel);
    }

    /// <summary>
    /// GET /Hotel/Data — Display valid hotel data with sorting/grouping.
    /// </summary>
    [HttpGet]
    public IActionResult Data(string? sortBy, string? groupBy)
    {
        var viewModel = new DataViewModel
        {
            SortBy = sortBy,
            GroupBy = groupBy
        };

        // Try to read from JSON first (our primary output format)
        var dataDir = Path.Combine(_environment.ContentRootPath, "Data");
        var jsonPath = Path.Combine(dataDir, "hotels.json");

        if (!System.IO.File.Exists(jsonPath))
        {
            return View(viewModel);
        }

        var jsonFormatter = _formatterFactory.GetFormatter("JSON");
        if (jsonFormatter == null) return View(viewModel);

        var hotels = jsonFormatter.Read(jsonPath);
        viewModel.SourceFile = "hotels.json";

        // Apply sorting
        hotels = sortBy?.ToLower() switch
        {
            "name" => hotels.OrderBy(h => h.Name).ToList(),
            "stars" => hotels.OrderByDescending(h => h.Stars).ToList(),
            "address" => hotels.OrderBy(h => h.Address).ToList(),
            "stars_asc" => hotels.OrderBy(h => h.Stars).ToList(),
            _ => hotels
        };

        // Apply grouping
        if (!string.IsNullOrWhiteSpace(groupBy))
        {
            viewModel.GroupedHotels = groupBy.ToLower() switch
            {
                "stars" => hotels.GroupBy(h => $"{h.Stars} Star(s)")
                    .OrderByDescending(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.ToList()),
                "country" => hotels.GroupBy(h => ExtractCountryHint(h.Address))
                    .OrderBy(g => g.Key)
                    .ToDictionary(g => g.Key, g => g.ToList()),
                _ => null
            };
        }
        else
        {
            viewModel.Hotels = hotels;
        }

        return View(viewModel);
    }

    /// <summary>
    /// Extracts a country hint from the address for grouping purposes.
    /// </summary>
    private static string ExtractCountryHint(string address)
    {
        if (string.IsNullOrWhiteSpace(address)) return "Unknown";

        // Try to detect country from address patterns
        if (address.Contains("APO") || address.Contains("FPO") || address.Contains("DPO"))
            return "Military";
        if (System.Text.RegularExpressions.Regex.IsMatch(address, @"\b[A-Z]{2}\s+\d{5}"))
            return "USA";
        if (address.Contains("(") && address.Contains(")") && 
            System.Text.RegularExpressions.Regex.IsMatch(address, @"\(\w{2}\)"))
            return "Italy";
        if (System.Text.RegularExpressions.Regex.IsMatch(address, @"\+33|France|chemin|boulevard|avenue|rue"))
            return "France";
        if (System.Text.RegularExpressions.Regex.IsMatch(address, @"\b\d{5}\s+[A-ZÄÖÜa-zäöüß]"))
            return "Germany";

        return "Other";
    }
}
