using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AgriStress.Infrastructure.DataAccess;
using AgriStress.Domain.Entities;
using AgriStress.Application.Interfaces;
using AgriStress.Application.DTOs;
using NetTopologySuite.Geometries;

namespace AgriStress.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FieldsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ISoilService _soilService;
    private readonly ILogger<FieldsController> _logger;

    public FieldsController(AppDbContext context, ISoilService soilService, ILogger<FieldsController> logger)
    {
        _context = context;
        _soilService = soilService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var fields = await _context.Fields.ToListAsync();
        return Ok(fields);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFieldRequest request)
    {
        _logger.LogInformation("Yeni tarla kaydı isteği alındı: {Name}, Lat: {Lat}, Lon: {Lon}", 
            request.Name, request.Latitude, request.Longitude);

        var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        var location = geometryFactory.CreatePoint(new Coordinate(request.Longitude, request.Latitude));

        // 1. SoilGrids servisinden toprak analizini çekiyoruz
        var soilData = await _soilService.GetSoilDataAsync(request.Latitude, request.Longitude);

        // 2. Verileri Field entity'sine bağlıyoruz
        var field = new Field
        {
            Name = request.Name,
            FarmerName = request.FarmerName,
            Location = location,
            SoilPh = soilData.SoilPh,
            Nitrogen = soilData.Nitrogen,
            OrganicCarbon = soilData.OrganicCarbon,
            SalinityEC = soilData.CationExchangeCapacity,
            CreatedAt = DateTime.UtcNow
        };

        _context.Fields.Add(field);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetAll), new { id = field.Id }, field);
    }
}