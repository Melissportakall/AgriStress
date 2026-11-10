using AgriStress.Infrastructure.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace AgriStress.API.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection"),
                x => x.UseNetTopologySuite()
            ));

        return services;
    }
}
