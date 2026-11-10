using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AgriStress.Application.Interfaces;
using AgriStress.Infrastructure.ExternalServices;

var services = new ServiceCollection();
services.AddLogging(builder => builder.AddSimpleConsole());
var provider = services.BuildServiceProvider();

var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger("SoilTester");

try
{
    using var httpClient = new HttpClient();
    var soilLogger = loggerFactory.CreateLogger<SoilGridsService>();
    var svc = new SoilGridsService(httpClient, soilLogger);

    logger.LogInformation("Calling GetSoilDataAsync for sample coordinates...");

    var result = await svc.GetSoilDataAsync(40.7654, 29.9408);

    var outJson = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
    Console.WriteLine("Result:\n" + outJson);
}
catch (Exception ex)
{
    logger.LogError(ex, "Error running soil test");
}
