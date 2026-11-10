namespace AgriStress.Application.DTOs;

public class CreateFieldRequest
{
    public string Name { get; set; } = string.Empty;
    public string FarmerName { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
