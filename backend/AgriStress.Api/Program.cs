using Microsoft.EntityFrameworkCore;
using AgriStress.Infrastructure.Data;
using AgriStress.Domain.Interfaces;
using AgriStress.Infrastructure.Services;
using NetTopologySuite.IO.Converters;

var builder = WebApplication.CreateBuilder(args);

// 1. PostGIS veritabanı bağlantısı
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite()
    ));

// 2. Toprak Analiz Servisi (HttpClient ile)
builder.Services.AddHttpClient<ISoilService, SoilGridsService>();

// 3. Mekânsal Kural Motoru Servisi (Yeni PostGIS Entegrasyonu)
builder.Services.AddScoped<ISpatialSoilService, SpatialSoilService>();

// 3. Controller ve GeoJSON serileştirme
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new GeoJsonConverterFactory());
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        // Veritabanını otomatik oluştur/güncelle
        context.Database.Migrate();

        var env = services.GetRequiredService<IWebHostEnvironment>();
        // Projenin kök dizinindeki (AgriStress) data/geotoprak_sinifi_32768.geojson dosyasının yolu
        var rootPath = Directory.GetParent(env.ContentRootPath)?.FullName ?? env.ContentRootPath;
        var geoJsonPath = Path.Combine(rootPath, "data", "geotoprak_sinifi_32768.geojson");

        await SoilDataSeeder.SeedAsync(context, geoJsonPath);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[AgriStress.Seeder] Beklenmeyen hata: {ex.Message}");
    }
}

app.Run();