using AgriStress.Domain.Interfaces;
using AgriStress.Domain.Models;
using AgriStress.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace AgriStress.Infrastructure.Services;

public class SpatialSoilService : ISpatialSoilService
{
    private readonly AppDbContext _context;

    public SpatialSoilService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<SoilAnalysisReport> AnalyzeLocationAsync(double latitude, double longitude)
    {
        var report = new SoilAnalysisReport
        {
            Latitude = latitude,
            Longitude = longitude,
            HasDataForLocation = false
        };

        // 1. Koordinatı PostGIS Point nesnesine çevir (X=Longitude, Y=Latitude)
        var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        var point = geometryFactory.CreatePoint(new Coordinate(longitude, latitude));

        // 2. Spatial Query: Hangi SoilZone poligonu bu noktayı içeriyor? (GIST İndeksi üzerinden)
        // DİKKAT: Veri setimiz (GeoJSON) UTM 35N / TUREF TM30 (EPSG:5254) metre cinsinden koordinatlara sahip.
        // Bu yüzden gönderilen Lat/Lon değerini ST_Transform ile 5254'e dönüştürüp sorguluyoruz.
        var zone = await _context.SoilZones
            .FromSqlRaw(@"
                SELECT * FROM ""SoilZones"" 
                WHERE ""Boundary"" IS NOT NULL 
                AND ST_Contains(
                    ""Boundary"", 
                    ST_SetSRID(ST_Transform(ST_SetSRID(ST_MakePoint({0}, {1}), 4326), 5254), 4326)
                )
                LIMIT 1", longitude, latitude)
            .FirstOrDefaultAsync();

        if (zone == null)
        {
            report.GeneralStressLevel = "Veri Yok";
            return report;
        }

        report.HasDataForLocation = true;
        
        // 3. Ham Verileri Map Et
        report.NitrogenValue = zone.ToplamAzot;
        report.NitrogenStatus = zone.AzotYeterlilik;
        report.PhosphorusValue = zone.Fosfor;
        report.PhosphorusStatus = zone.FosforYeterlilik;
        report.PotassiumValue = zone.Potasyum;
        report.PotassiumStatus = zone.PotasyumYeterlilik;
        report.OrganicMatterPercentage = zone.OrganikMaddeYuzdesi;
        report.SoilTexture = zone.UstTekstur;
        
        report.RecommendedFruitsVegetables = zone.MeyveSebzeUygunlugu;
        report.RecommendedDryCrops = zone.KuruTarlaUygunlugu;
        report.RecommendedIrrigatedCrops = zone.SuluTarlaUygunlugu;

        // 4. Kural Motoru (Rule Engine)
        var stressPoints = 0;

        if (!string.IsNullOrEmpty(zone.AzotYeterlilik) && 
            (zone.AzotYeterlilik.Contains("Az", StringComparison.OrdinalIgnoreCase) || 
             zone.AzotYeterlilik.Contains("Yetersiz", StringComparison.OrdinalIgnoreCase)))
        {
            report.Deficiencies.Add("Azot Eksikliği (N) - Kök ve Gövde gelişimini yavaşlatır.");
            stressPoints++;
        }

        if (!string.IsNullOrEmpty(zone.FosforYeterlilik) && 
            zone.FosforYeterlilik.Contains("Az", StringComparison.OrdinalIgnoreCase))
        {
            report.Deficiencies.Add("Fosfor Eksikliği (P) - Çiçeklenme ve tohum tutumunu olumsuz etkiler.");
            stressPoints++;
        }
        
        if (zone.OrganikMaddeYuzdesi < 2.0)
        {
            report.Deficiencies.Add("Düşük Organik Madde - Toprağın su tutma kapasitesi yetersiz olabilir.");
            stressPoints++;
        }

        // Stres Puanına Göre Sınıflandırma
        if (stressPoints == 0)
        {
            report.GeneralStressLevel = "Mükemmel (Stres Yok)";
        }
        else if (stressPoints == 1)
        {
            report.GeneralStressLevel = "Düşük Stres - Belirli takviyelerle verim artırılabilir";
        }
        else if (stressPoints == 2)
        {
            report.GeneralStressLevel = "Orta Stres - Gübreleme şart";
        }
        else
        {
            report.GeneralStressLevel = "Yüksek Stres - Toprak ıslahı gerekli";
        }

        return report;
    }
}
