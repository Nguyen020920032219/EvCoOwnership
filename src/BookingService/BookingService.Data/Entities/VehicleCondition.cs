namespace BookingService.Data.Entities;

public class VehicleCondition
{
    public int VehicleConditionId { get; set; }

    public int CheckInOutId { get; set; }

    public string Name { get; set; } = null!;

    public string? Detail { get; set; }

    public int VehicleId { get; set; }

    public virtual BookingCheckInOut CheckInOut { get; set; } = null!;
}