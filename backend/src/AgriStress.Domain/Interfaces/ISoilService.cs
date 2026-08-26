namespace AgriStress.Domain.Interfaces;

public class SoilDataResult
{
    public double? SoilPh { get; set; }
    public double? Nitrogen { get; set; }
    public double? OrganicCarbon { get; set; }
    public double? CationExchangeCapacity { get; set; }
}

public interface ISoilService
{
    Task<SoilDataResult> GetSoilDataAsync(double latitude, double longitude);
}