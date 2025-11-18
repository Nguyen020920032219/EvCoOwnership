namespace BookingService.Data.Entities;

public class BookingCheckInOut
{
    public int CheckInOutId { get; set; }

    public DateTime DateTime { get; set; }

    public int Type { get; set; }

    public int BookingId { get; set; }

    public virtual VehicleBooking Booking { get; set; } = null!;

    public virtual ICollection<VehicleCondition> VehicleConditions { get; set; } = new List<VehicleCondition>();
}