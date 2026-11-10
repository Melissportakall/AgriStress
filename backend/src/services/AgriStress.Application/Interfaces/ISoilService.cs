using AgriStress.Application.DTOs;

namespace AgriStress.Application.Interfaces;

public interface ISoilService
{
    Task<SoilDataResult> GetSoilDataAsync(double latitude, double longitude);
}
