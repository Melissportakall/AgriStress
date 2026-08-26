using AgriStress.Domain.Models;

namespace AgriStress.Domain.Interfaces;

public interface ISpatialSoilService
{
    Task<SoilAnalysisReport> AnalyzeLocationAsync(double latitude, double longitude);
}
