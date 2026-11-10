using AgriStress.Application.Interfaces;
using AgriStress.Application.Services;
using AgriStress.Infrastructure.ExternalServices;
using AgriStress.Infrastructure.Services;

namespace AgriStress.API.Configurations;

public static class ServiceDependenciesConfiguration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Application Katmanı Servisleri (Kural Motorları)
        services.AddScoped<IAgronomyRuleEngine, AgronomyRuleEngine>();

        // Infrastructure Katmanı Servisleri (HTTP Client Dış Servisler)
        services.AddHttpClient<ISoilService, SoilGridsService>();
        services.AddHttpClient<IWeatherService, OpenMeteoWeatherService>();

        // Mekânsal Toprak Teşhis Servisi
        services.AddScoped<ISpatialSoilService, SpatialSoilService>();

        return services;
    }
}
