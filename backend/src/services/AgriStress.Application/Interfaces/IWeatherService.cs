using AgriStress.Application.DTOs;

namespace AgriStress.Application.Interfaces;

public interface IWeatherService
{
    Task<WeatherData?> GetCurrentWeatherAsync(double lat, double lon);
}
