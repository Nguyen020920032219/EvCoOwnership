namespace BookingService.Data.Entities;

public class VehicleBooking
{
    public int BookingId { get; set; }

    public int VehicleId { get; set; }

    public int UserId { get; set; }

    public int CoOwnerGroupId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int Status { get; set; }

    public decimal? DistanceKm { get; set; }

    public decimal? EnergyKwh { get; set; }

    public virtual ICollection<BookingCheckInOut> BookingCheckInOuts { get; set; } = new List<BookingCheckInOut>();
}