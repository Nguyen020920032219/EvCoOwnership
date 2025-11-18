namespace BookingService.Business.Models;

public class VehicleConditionInput
{
    public string Name { get; set; } = string.Empty; // Vỏ, Lốp, Nội thất...
    public string Detail { get; set; } = string.Empty; // "OK", "Bẩn", "Xước"...
}