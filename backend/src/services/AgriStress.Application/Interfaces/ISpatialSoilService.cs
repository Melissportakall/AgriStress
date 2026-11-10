using AgriStress.Application.DTOs;

namespace AgriStress.Application.Interfaces;

public interface ISpatialSoilService
{
    Task<SoilAnalysisReport> AnalyzeLocationAsync(double latitude, double longitude);
}
