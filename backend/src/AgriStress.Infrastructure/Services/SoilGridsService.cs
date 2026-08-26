using System.Globalization;
using System.Text.Json;
using System.Threading;
using System.Collections.Concurrent;
using Polly;
using Polly.CircuitBreaker;
using AgriStress.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AgriStress.Infrastructure.Services;

public class SoilGridsService : ISoilService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<SoilGridsService> _logger;
    // Simple in-memory cache to reduce repeated slow requests to SoilGrids
    private static readonly ConcurrentDictionary<string, SoilDataResult> _cache = new();

    public SoilGridsService(HttpClient httpClient, ILogger<SoilGridsService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<SoilDataResult> GetSoilDataAsync(double latitude, double longitude)
    {
        var cacheKey = string.Format(CultureInfo.InvariantCulture, "{0:F6},{1:F6}", latitude, longitude);

        if (_cache.TryGetValue(cacheKey, out var cached))
        {
            _logger.LogDebug("Soil data cache hit for {Key}", cacheKey);
            return cached;
        }

        var result = new SoilDataResult();

        var latStr = latitude.ToString(CultureInfo.InvariantCulture);
        var lonStr = longitude.ToString(CultureInfo.InvariantCulture);

        // İlk tercih: ISRIC SoilGrids (daha zengin toprak kimyası verisi sağlayabilir)
        // Denemeler için daha hafif bir sorgu: istenen property'leri belirterek gönderiyoruz
        var soilGridsBase = "https://rest.isric.org/soilgrids/v2.0/properties/query";
        var simpleProps = new[] { "phh2o", "soc", "cec", "nitrogen" };
        var soilGridsUrlSimple = soilGridsBase + $"?lat={latStr}&lon={lonStr}" + string.Concat(simpleProps.Select(p => "&property=" + p));
        var soilGridsUrlFull = soilGridsBase + $"?lat={latStr}&lon={lonStr}";

        // Polly policies: retry with backoff + circuit breaker
        var retryPolicy = Policy.Handle<Exception>().WaitAndRetryAsync(new[] {
            TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(4), TimeSpan.FromSeconds(8)
        }, (ex, ts) => _logger.LogWarning(ex, "SoilGrids retry after {Delay}", ts));

        var circuitBreakerPolicy = Policy.Handle<Exception>().CircuitBreakerAsync(2, TimeSpan.FromSeconds(30),
            onBreak: (ex, ts) => _logger.LogWarning(ex, "Circuit opened for SoilGrids"),
            onReset: () => _logger.LogInformation("Circuit reset for SoilGrids"));

        var policyWrap = Policy.WrapAsync(retryPolicy, circuitBreakerPolicy);

        try
        {
            // Önce basit property filtreli isteği deneyelim
            _logger.LogInformation("SoilGrids basit sorgu deneniyor: {Url}", soilGridsUrlSimple);

            var response = await policyWrap.ExecuteAsync(async ct =>
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cts.CancelAfter(TimeSpan.FromSeconds(60));
                using var request = new HttpRequestMessage(HttpMethod.Get, soilGridsUrlSimple);
                request.Headers.Add("User-Agent", "AgriStressApp/1.0");
                return await _httpClient.SendAsync(request, cts.Token);
            }, CancellationToken.None);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;

                result.SoilPh = TryFindNumericProperty(root, "phh2o");
                result.Nitrogen = TryFindNumericProperty(root, "nitrogen") ?? TryFindNumericProperty(root, "n");
                result.OrganicCarbon = TryFindNumericProperty(root, "soc") ?? TryFindNumericProperty(root, "organic_carbon") ?? TryFindNumericProperty(root, "organiccarbon");
                result.CationExchangeCapacity = TryFindNumericProperty(root, "cec") ?? TryFindNumericProperty(root, "cation_exchange_capacity");

                if (result.SoilPh.HasValue || result.Nitrogen.HasValue || result.OrganicCarbon.HasValue || result.CationExchangeCapacity.HasValue)
                {
                    _logger.LogInformation("SoilGrids (simple) veri bulundu ve parse edildi: {Lat},{Lon}", latitude, longitude);
                    _cache[cacheKey] = result;
                    return result;
                }
            }

            // Eğer basit sorgu başarısızsa, tam sorguyu dene
            _logger.LogInformation("SoilGrids tam sorgu denenecek: {Url}", soilGridsUrlFull);
            response = await policyWrap.ExecuteAsync(async ct =>
            {
                using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
                cts.CancelAfter(TimeSpan.FromSeconds(60));
                using var request = new HttpRequestMessage(HttpMethod.Get, soilGridsUrlFull);
                request.Headers.Add("User-Agent", "AgriStressApp/1.0");
                return await _httpClient.SendAsync(request, cts.Token);
            }, CancellationToken.None);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;

                result.SoilPh = TryFindNumericProperty(root, "phh2o");
                result.Nitrogen = TryFindNumericProperty(root, "nitrogen") ?? TryFindNumericProperty(root, "n");
                result.OrganicCarbon = TryFindNumericProperty(root, "soc") ?? TryFindNumericProperty(root, "organic_carbon") ?? TryFindNumericProperty(root, "organiccarbon");
                result.CationExchangeCapacity = TryFindNumericProperty(root, "cec") ?? TryFindNumericProperty(root, "cation_exchange_capacity");

                if (result.SoilPh.HasValue || result.Nitrogen.HasValue || result.OrganicCarbon.HasValue || result.CationExchangeCapacity.HasValue)
                {
                    _logger.LogInformation("SoilGrids (full) veri bulundu ve parse edildi: {Lat},{Lon}", latitude, longitude);
                    _cache[cacheKey] = result;
                    return result;
                }
            }
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogWarning(ex, "SoilGrids circuit açık; fallback uygulanacak.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "SoilGrids sorgusu sırasında beklenmeyen hata");
        }

        // Eğer SoilGrids ile anlamlı veri alınamadıysa, mevcut Open-Meteo tabanlı kestirim mantığını kullan (fallback)
        try
        {
            var openMeteoUrl = $"https://api.open-meteo.com/v1/forecast?latitude={latStr}&longitude={lonStr}&current=soil_temperature_0_to_7cm,soil_moisture_0_to_7cm";
            _logger.LogInformation("Open-Meteo fallback sorgulanıyor: {Url}", openMeteoUrl);

            using var request = new HttpRequestMessage(HttpMethod.Get, openMeteoUrl);
            request.Headers.Add("User-Agent", "AgriStressApp/1.0");

            var response = await _httpClient.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(jsonString);
                var root = doc.RootElement;

                if (root.TryGetProperty("current", out var current))
                {
                    double? soilTemp = current.TryGetProperty("soil_temperature_0_to_7cm", out var t) ? t.GetDouble() : null;
                    double? soilMoisture = current.TryGetProperty("soil_moisture_0_to_7cm", out var m) ? m.GetDouble() : null;

                    var moistureFactor = (soilMoisture ?? 0.25);

                    result.SoilPh ??= Math.Round(7.25 + (moistureFactor * 0.4), 2);
                    result.Nitrogen ??= Math.Round(1.65 + (moistureFactor * 1.5), 2);
                    result.OrganicCarbon ??= Math.Round(18.2 + (moistureFactor * 12.0), 2);
                    result.CationExchangeCapacity ??= Math.Round(24.0 + (moistureFactor * 10.0), 2);

                    _logger.LogInformation("Open-Meteo fallback ile toprak kestirimi yapıldı -> Nem: {Moisture} m³/m³, pH: {Ph}", soilMoisture, result.SoilPh);
                    _cache[cacheKey] = result;
                    return result;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Open-Meteo fallback sırasında hata oluştu.");
        }

        // Herhangi bir veri alınamadıysa boş sonucu cache'leyip dön
        _cache[cacheKey] = result;
        return result;
    }

    private static double? TryFindNumericProperty(JsonElement element, string propertyName)
    {
        if (element.ValueKind == JsonValueKind.Object)
        {
            foreach (var prop in element.EnumerateObject())
            {
                if (string.Equals(prop.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                {
                    var found = FindFirstNumber(prop.Value);
                    if (found.HasValue) return found.Value;
                }

                var nested = TryFindNumericProperty(prop.Value, propertyName);
                if (nested.HasValue) return nested.Value;
            }
        }
        else if (element.ValueKind == JsonValueKind.Array)
        {
            foreach (var item in element.EnumerateArray())
            {
                var nested = TryFindNumericProperty(item, propertyName);
                if (nested.HasValue) return nested.Value;
            }
        }

        return null;
    }

    private static double? FindFirstNumber(JsonElement el)
    {
        if (el.ValueKind == JsonValueKind.Number)
            return el.GetDouble();

        if (el.ValueKind == JsonValueKind.Object)
        {
            foreach (var p in el.EnumerateObject())
            {
                var v = FindFirstNumber(p.Value);
                if (v.HasValue) return v.Value;
            }
        }

        if (el.ValueKind == JsonValueKind.Array)
        {
            foreach (var it in el.EnumerateArray())
            {
                var v = FindFirstNumber(it);
                if (v.HasValue) return v.Value;
            }
        }

        return null;
    }
}