namespace AgriStress.Domain.Models;

public class SoilAnalysisReport
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public bool HasDataForLocation { get; set; }
    
    public string GeneralStressLevel { get; set; } = "Bilinmiyor";
    public List<string> Deficiencies { get; set; } = new();
    
    // Ham Değerler
    public double? NitrogenValue { get; set; }
    public string? NitrogenStatus { get; set; }
    
    public double? PhosphorusValue { get; set; }
    public string? PhosphorusStatus { get; set; }
    
    public double? PotassiumValue { get; set; }
    public string? PotassiumStatus { get; set; }
    
    public double? OrganicMatterPercentage { get; set; }
    
    public string? SoilTexture { get; set; } // TeksUst
    
    // Ürün Uygunluğu (Veri setinden gelen hazır öneriler)
    public string? RecommendedFruitsVegetables { get; set; }
    public string? RecommendedDryCrops { get; set; }
    public string? RecommendedIrrigatedCrops { get; set; }
}
