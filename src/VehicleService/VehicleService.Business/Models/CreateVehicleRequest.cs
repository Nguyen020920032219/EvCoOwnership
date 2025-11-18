namespace VehicleService.Business.Models;

public class CreateVehicleRequest
{
    public string LicensePlate { get; set; } = string.Empty;
    public string Vin { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Color { get; set; } = string.Empty;
    public decimal BatteryCapacityKwh { get; set; }
    public string ChargingPortType { get; set; } = "CCS2";
    public int CoOwnerGroupId { get; set; }
    public decimal PurchasePrice { get; set; }
}