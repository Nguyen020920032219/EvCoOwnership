namespace VehicleService.Business.Models;

public class VehicleDto
{
    public int VehicleId { get; set; }
    public string LicensePlate { get; set; } = string.Empty;
    public string Vin { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public decimal? BatteryCapacityKwh { get; set; }
    public string? Status { get; set; }
    public int CoOwnerGroupId { get; set; }
}