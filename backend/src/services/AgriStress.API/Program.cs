using AgriStress.API.Configurations;
using AgriStress.API.Middlewares;
using AgriStress.Infrastructure.DataAccess;
using AgriStress.Infrastructure.Seeder;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.IO.Converters;

var builder = WebApplication.CreateBuilder(args);

// 1. Veritabanı ve PostGIS Yapılandırması (Configurations/DatabaseConfiguration.cs)
builder.Services.AddDatabaseConfiguration(builder.Configuration);

// 2. Application ve Infrastructure Servis Bağımlılıkları (Configurations/ServiceDependenciesConfiguration.cs)
builder.Services.AddApplicationServices();

// 3. Controller ve GeoJSON Serileştirme
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
    });

// 4. CORS Yapılandırması (Configurations/CorsConfiguration.cs)
builder.Services.AddCorsConfiguration();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Global Exception Handling Middleware
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(CorsConfiguration.PolicyName);

app.UseAuthorization();
app.MapControllers();

// Otomatik Veritabanı Migration ve GeoJSON Seed İşlemi
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();

        var env = services.GetRequiredService<IWebHostEnvironment>();
        var geoJsonPath = FindGeoJsonPath(env.ContentRootPath);

        await SoilDataSeeder.SeedAsync(context, geoJsonPath);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[AgriStress.Seeder] Beklenmeyen hata: {ex.Message}");
    }
}

app.Run();

static string FindGeoJsonPath(string startPath)
{
    var current = new DirectoryInfo(startPath);
    while (current != null)
    {
        var testPath = Path.Combine(current.FullName, "data", "geotoprak_sinifi_32768.geojson");
        if (File.Exists(testPath)) return testPath;
        current = current.Parent;
    }
    return Path.Combine(startPath, "data", "geotoprak_sinifi_32768.geojson");
}