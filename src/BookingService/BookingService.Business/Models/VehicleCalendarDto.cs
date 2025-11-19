namespace BookingService.Business.Models;

public class VehicleCalendarDto
{
    public int BookingId { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = string.Empty; // "Upcoming", "InProgress", "Completed"
    
    // Có thể thêm thông tin người đặt nếu muốn hiển thị công khai (tùy policy nhóm)
    // public int BookedByUserId { get; set; } 
}