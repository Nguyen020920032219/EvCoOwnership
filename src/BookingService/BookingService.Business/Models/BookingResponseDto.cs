namespace BookingService.Business.Models;

public class BookingResponseDto
{
    public int BookingId { get; set; }
    public int VehicleId { get; set; }
    public int UserId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
}