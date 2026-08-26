using NetTopologySuite.Geometries;

namespace AgriStress.Domain.Entities;

public class Field
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string FarmerName { get; set; } = string.Empty;

    // Haritada çiftçinin işaretlediği nokta (Pin)
    public Point? Location { get; set; }

    // Çizilen tarla sınır poligonu
    public Polygon? Boundary { get; set; }

    // Toprak Kimyası (SoilGrids)
    public double? Nitrogen { get; set; }
    public double? SoilPh { get; set; }
    public double? OrganicCarbon { get; set; }
    public double? SalinityEC { get; set; }

    // Uydu İndeksleri (Sentinel-2)
    public double? LastNdvi { get; set; }
    public double? LastNdwi { get; set; }
    public string? StressLevel { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}