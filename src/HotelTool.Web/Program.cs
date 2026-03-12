using HotelTool.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Register services
builder.Services.AddSingleton<CsvReaderService>();
builder.Services.AddSingleton<HotelValidator>();

// Register output formatters (extensible — add new formatters here)
builder.Services.AddSingleton<IOutputFormatter, JsonOutputFormatter>();
builder.Services.AddSingleton<IOutputFormatter, XmlOutputFormatter>();

// Register formatter factory
builder.Services.AddSingleton<OutputFormatterFactory>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Hotel}/{action=Upload}/{id?}");

app.Run();
