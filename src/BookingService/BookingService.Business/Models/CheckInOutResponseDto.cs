namespace BookingService.Business.Models;

public class CheckInOutResponseDto
{
    public int CheckInOutId { get; set; }
    public string Type { get; set; } = string.Empty; // "CheckIn" hoặc "CheckOut"
    public DateTime Time { get; set; }
}