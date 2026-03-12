# 🏨 HotelTool

A web-based hotel data management tool built with **ASP.NET Core MVC (.NET 9)**. Upload hotel CSV files, validate entries against defined rules, export valid data in multiple formats (JSON & XML), and visualize the results — all from a clean, dark-themed web UI.

> **Context:** This project was developed as part of a **backend developer internship interview task**. The focus is on **code quality, extensibility, and software architecture** rather than raw performance.

---

## 📋 Task Requirements

| # | Requirement | Status |
|---|---|:---:|
| 1 | Simple web UI to upload the CSV file | ✅ |
| 2 | Read data from the given CSV file (`hotels.csv`) — first line is a header | ✅ |
| 3 | Validate data (UTF-8 name, valid URL, rating 0–5) | ✅ |
| 4 | Write valid data in two formats (XML, JSON, YAML, HTML, SQLite, or custom) | ✅ JSON + XML |
| 5 | Simple web UI to visualize the valid data | ✅ |
| 6 | Make the tool extensible to new output formats | ✅ Strategy + Factory Pattern |
| 7 | Code quality over performance (readability, architecture) | ✅ |
| 8 | Unit tests | ✅ 37 test cases |
| 9 | Options to sort / group the data before writing | ✅ Sort & Group |
| 10 | .NET backend (optional JS frontend framework) | ✅ Pure ASP.NET Core MVC |

---

## 🏗️ Architecture

```
HotelTool/
├── src/
│   └── HotelTool.Web/              # ASP.NET Core MVC Application
│       ├── Controllers/
│       │   └── HotelController.cs   # Upload (GET/POST) + Data (GET)
│       ├── Models/
│       │   ├── Hotel.cs             # Core domain model
│       │   ├── HotelValidationResult.cs
│       │   ├── UploadViewModel.cs
│       │   └── DataViewModel.cs
│       ├── Services/
│       │   ├── CsvReaderService.cs      # CSV parsing (quoted fields, multi-encoding)
│       │   ├── HotelValidator.cs        # Validation rules engine
│       │   ├── IOutputFormatter.cs      # Output format interface (Strategy)
│       │   ├── JsonOutputFormatter.cs   # JSON implementation
│       │   ├── XmlOutputFormatter.cs    # XML implementation
│       │   └── OutputFormatterFactory.cs # Factory for format resolution
│       ├── Views/
│       │   ├── Hotel/
│       │   │   ├── Upload.cshtml    # File upload + results UI
│       │   │   ├── Data.cshtml      # Data visualization UI
│       │   │   └── _HotelTable.cshtml # Reusable table partial
│       │   └── Shared/
│       │       └── _Layout.cshtml   # Main layout
│       ├── wwwroot/css/
│       │   └── site.css             # Consolidated stylesheet
│       ├── Data/                    # Runtime output directory
│       └── Program.cs              # DI configuration & pipeline
│
└── tests/
    └── HotelTool.Tests/            # xUnit Test Project
        ├── CsvReaderServiceTests.cs
        ├── HotelValidatorTests.cs
        └── OutputFormatterTests.cs
```

### Design Patterns

| Pattern | Where | Purpose |
|---|---|---|
| **Strategy** | `IOutputFormatter` → `JsonOutputFormatter`, `XmlOutputFormatter` | Each output format is an independent, interchangeable implementation |
| **Factory** | `OutputFormatterFactory` | Resolves registered formatters by name; provides `GetAllFormatters()` |
| **Dependency Injection** | `Program.cs` | All services registered as singletons; controller receives dependencies via constructor |
| **MVC** | Controllers, Views, Models | Clear separation of concerns |
| **Open/Closed Principle** | Adding new formatters | New formats require zero changes to existing code |

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)

### Run the Application

```bash
dotnet run --project src/HotelTool.Web/
```

The app starts at **http://localhost:5049**

### Run Tests

```bash
dotnet test
```

---

## 📖 Features

### 1. CSV Upload

- Upload a `hotels.csv` file through the web UI
- CSV parser handles quoted fields with commas, escaped quotes, and mixed line endings (`\r\n`, `\n`, `\r`)
- First row is treated as header

### 2. Validation Rules

| Rule | Implementation |
|---|---|
| **Hotel name** | Must contain only valid UTF-8 characters — rejects Unicode replacement character (`U+FFFD`) and control characters (except tab, newline). Empty/whitespace names are rejected. |
| **Hotel URL** | Must be an absolute URI with `http` or `https` scheme, non-empty host, and at least one dot in the hostname. |
| **Hotel rating** | Integer between 0 and 5 (inclusive). Negative values and values above 5 are rejected. |

Invalid entries are displayed in the UI with line numbers and specific error messages.

### 3. Output Formats

Valid hotels are automatically exported to both formats in the `Data/` directory:

- **JSON** — Indented, camelCase, full Unicode support (`System.Text.Json`)
- **XML** — UTF-8 encoded, root element `<Hotels>` (`XmlSerializer`)

### 4. Data Visualization

The Data page reads valid hotels from the generated JSON file and displays them in a sortable, groupable table.

**Sort options:**
- Name (A–Z)
- Stars (High → Low)
- Stars (Low → High)
- Address (A–Z)

**Group options:**
- By Stars
- By Country (inferred from address)

### 5. Extensibility

Adding a new output format (e.g., YAML) requires only two steps:

1. Create a class implementing `IOutputFormatter`:

```csharp
public class YamlOutputFormatter : IOutputFormatter
{
    public string FormatName => "YAML";
    public string FileExtension => ".yaml";
    public void Write(List<Hotel> hotels, string filePath) { /* ... */ }
    public List<Hotel> Read(string filePath) { /* ... */ }
}
```

2. Register it in `Program.cs`:

```csharp
builder.Services.AddSingleton<IOutputFormatter, YamlOutputFormatter>();
```

No other code changes needed — the factory and controller automatically pick up new formatters.

---

## 🧪 Tests

**37 test cases** across 3 test files using **xUnit 2.9.2**:

| Test File | Tests | Coverage |
|---|---|---|
| `CsvReaderServiceTests` | 8 | CSV parsing, quoted fields, empty files, various line endings |
| `HotelValidatorTests` | 12 (10 Facts + 2 Theories with 13 inline cases) | All validation rules, edge cases, Unicode names, boundary values |
| `OutputFormatterTests` | 6 | JSON/XML write & read, metadata, empty lists, factory resolution |

```bash
# Run all tests
dotnet test

# Run with detailed output
dotnet test --verbosity normal

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

---

## 🛠️ Tech Stack

| Layer | Technology |
|---|---|
| **Backend** | ASP.NET Core MVC (.NET 9) |
| **Frontend** | Razor Views + Custom CSS (dark theme) |
| **Serialization** | `System.Text.Json`, `System.Xml.Serialization` |
| **Testing** | xUnit 2.9.2 + Coverlet |
| **Dependencies** | Zero third-party NuGet packages (web project) |

---

## 📄 License

This project was created for internship interview purposes.
