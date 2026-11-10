namespace AgriStress.API.Configurations;

public static class CorsConfiguration
{
    public const string PolicyName = "AllowFrontend";

    public static IServiceCollection AddCorsConfiguration(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName,
                policy =>
                {
                    policy.WithOrigins("http://localhost:5173", "http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
        });

        return services;
    }
}
