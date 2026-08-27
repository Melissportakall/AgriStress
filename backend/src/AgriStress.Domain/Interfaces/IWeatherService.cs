using AgriStress.Domain.Models;

namespace AgriStress.Domain.Interfaces;

public interface IWeatherService
{
    Task<WeatherData?> GetCurrentWeatherAsync(double lat, double lon);
}
