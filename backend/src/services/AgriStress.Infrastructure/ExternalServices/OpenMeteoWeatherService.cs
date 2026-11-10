using System.Text.Json;
using System.Text.Json.Serialization;
using AgriStress.Application.DTOs;
using AgriStress.Application.Interfaces;

namespace AgriStress.Infrastructure.ExternalServices;

public class OpenMeteoWeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;

    public OpenMeteoWeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherData?> GetCurrentWeatherAsync(double lat, double lon)
    {
        try
        {
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={lat}&longitude={lon}&current_weather=true";
            var response = await _httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<OpenMeteoResponse>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (result?.Current_Weather != null)
            {
                return new WeatherData
                {
                    Temperature = result.Current_Weather.Temperature,
                    WindSpeed = result.Current_Weather.WindSpeed,
                    Time = result.Current_Weather.Time,
                    WeatherCode = result.Current_Weather.WeatherCode
                };
            }

            return null;
        }
        catch
        {
            // Hava durumu API hatası ana akışı bozmasın
            return null;
        }
    }

    private class OpenMeteoResponse
    {
        public CurrentWeather? Current_Weather { get; set; }
    }

    private class CurrentWeather
    {
        public double Temperature { get; set; }
        public double WindSpeed { get; set; }
        [JsonPropertyName("weathercode")]
        public int WeatherCode { get; set; }
        public string? Time { get; set; }
    }
}
