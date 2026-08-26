using AgriStress.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO.Converters;
using System.Text.Json;

namespace AgriStress.Infrastructure.Data;

public static class SoilDataSeeder
{
    public static async Task SeedAsync(AppDbContext context, string geoJsonFilePath)
    {
        // Sadece tablo boşsa çalıştır (1 kez)
        if (await context.SoilZones.AnyAsync())
            return;

        if (!File.Exists(geoJsonFilePath))
        {
            Console.WriteLine($"[AgriStress.Seeder] HATA: GeoJSON dosyası bulunamadı: {geoJsonFilePath}");
            return;
        }

        Console.WriteLine($"[AgriStress.Seeder] GeoJSON dosyası okunuyor: {geoJsonFilePath} Lütfen bekleyin...");

        var jsonOptions = new JsonSerializerOptions();
        jsonOptions.Converters.Add(new GeoJsonConverterFactory());

        using var stream = File.OpenRead(geoJsonFilePath);
        var featureCollection = await JsonSerializer.DeserializeAsync<FeatureCollection>(stream, jsonOptions);

        if (featureCollection == null || featureCollection.Count == 0)
            return;

        Console.WriteLine($"[AgriStress.Seeder] Toplam {featureCollection.Count} adet toprak poligonu veritabanına aktarılıyor...");

        var soilZones = new List<SoilZone>();

        foreach (var feature in featureCollection)
        {
            var geom = feature.Geometry;
            MultiPolygon? multiPolygon = null;

            if (geom is Polygon poly)
            {
                multiPolygon = new MultiPolygon(new[] { poly });
            }
            else if (geom is MultiPolygon mp)
            {
                multiPolygon = mp;
            }
            
            if (multiPolygon != null)
            {
                multiPolygon.SRID = 4326; 
            }

            var attributes = feature.Attributes;
            
            var soilZone = new SoilZone
            {
                Boundary = multiPolygon,
                ToplamAzot = GetDouble(attributes, "topn"),
                AzotYeterlilik = GetString(attributes, "topn_yet"),
                Fosfor = GetDouble(attributes, "p205"),
                FosforYeterlilik = GetString(attributes, "p205_yet"),
                Potasyum = GetDouble(attributes, "k2o"),
                PotasyumYeterlilik = GetString(attributes, "k2o_yet"),
                OrganikMaddeYuzdesi = GetDouble(attributes, "org_mad_y"),
                DemirPpm = GetDouble(attributes, "fe_ppm"),
                DemirYeterlilik = GetString(attributes, "fe_yet"),
                CinkoPpm = GetDouble(attributes, "zn_ppm"),
                CinkoYeterlilik = GetString(attributes, "zn_yet"),
                ManganPpm = GetDouble(attributes, "mn_ppm"),
                BakirPpm = GetDouble(attributes, "cu_ppm"),
                BorPpm = GetDouble(attributes, "b_ppm"),
                UstTekstur = GetString(attributes, "teks_ust"),
                AltTekstur = GetString(attributes, "teks_alt"),
                EgimSinifi = GetString(attributes, "egim_sin"),
                DerinlikSinifi = GetString(attributes, "der_sin"),
                AraziKullanimKabiliyeti = GetString(attributes, "akk_sin_t"),
                MeyveSebzeUygunlugu = GetString(attributes, "smeyseb_aciklama"),
                SuluTarlaUygunlugu = GetString(attributes, "starla_aciklama"),
                KuruTarlaUygunlugu = GetString(attributes, "ktarla_aciklama")
            };
            
            soilZones.Add(soilZone);
        }

        await context.SoilZones.AddRangeAsync(soilZones);
        await context.SaveChangesAsync();
        
        Console.WriteLine("[AgriStress.Seeder] Veri aktarımı başarıyla tamamlandı!");
    }

    private static double? GetDouble(IAttributesTable attributes, string key)
    {
        if (attributes != null && attributes.Exists(key) && attributes[key] != null)
        {
            var val = attributes[key];
            if (val is double d) return d;
            if (val is int i) return i;
            if (val is long l) return l;
            if (val is decimal dec) return (double)dec;
            if (double.TryParse(val.ToString(), out var result)) return result;
        }
        return null;
    }

    private static string? GetString(IAttributesTable attributes, string key)
    {
        if (attributes != null && attributes.Exists(key) && attributes[key] != null)
        {
            return attributes[key].ToString();
        }
        return null;
    }
}
