namespace BookingService.Business.Models;

public class CheckInOutRequest
{
    public int BookingId { get; set; }
    
    // List tình trạng xe (ví dụ: "Vỏ xe" -> "Xước nhẹ")
    public List<VehicleConditionInput> Conditions { get; set; } = new();
    
    // Dành riêng cho Checkout
    public decimal? DistanceKm { get; set; } // Số km đã đi
    public decimal? EnergyKwh { get; set; }  // Số điện tiêu thụ
}