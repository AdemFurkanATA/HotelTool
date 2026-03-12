namespace HotelTool.Web.Services;

/// <summary>
/// Factory for retrieving output formatters by name.
/// All IOutputFormatter implementations registered in DI are automatically available.
/// 
/// To add a new format:
/// 1. Create a class that implements IOutputFormatter
/// 2. Register it in Program.cs: builder.Services.AddSingleton&lt;IOutputFormatter, YourFormatter&gt;()
/// 3. It will be automatically available through this factory
/// </summary>
public class OutputFormatterFactory
{
    private readonly Dictionary<string, IOutputFormatter> _formatters;

    public OutputFormatterFactory(IEnumerable<IOutputFormatter> formatters)
    {
        _formatters = formatters.ToDictionary(
            f => f.FormatName.ToUpperInvariant(),
            f => f);
    }

    /// <summary>
    /// Gets a formatter by name (case-insensitive).
    /// </summary>
    public IOutputFormatter? GetFormatter(string formatName)
    {
        return _formatters.GetValueOrDefault(formatName.ToUpperInvariant());
    }

    /// <summary>
    /// Gets all registered formatters.
    /// </summary>
    public IReadOnlyCollection<IOutputFormatter> GetAllFormatters()
    {
        return _formatters.Values.ToList().AsReadOnly();
    }
}
