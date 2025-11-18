namespace BookingService.Business.Models;

public class BookingRequestDto
{
    public int VehicleId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}