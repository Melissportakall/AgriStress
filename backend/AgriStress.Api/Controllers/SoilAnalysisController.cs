using AgriStress.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AgriStress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SoilAnalysisController : ControllerBase
{
    private readonly ISpatialSoilService _spatialSoilService;

    public SoilAnalysisController(ISpatialSoilService spatialSoilService)
    {
        _spatialSoilService = spatialSoilService;
    }

    [HttpGet("analyze")]
    public async Task<IActionResult> Analyze([FromQuery] double lat, [FromQuery] double lon)
    {
        var result = await _spatialSoilService.AnalyzeLocationAsync(lat, lon);
        
        if (!result.HasDataForLocation)
        {
            return NotFound(new { 
                Message = "Seçilen konum için toprak verisi bulunamadı. Lütfen analiz havzası içerisinde bir koordinat seçin.", 
                Result = result 
            });
        }

        return Ok(result);
    }
}
