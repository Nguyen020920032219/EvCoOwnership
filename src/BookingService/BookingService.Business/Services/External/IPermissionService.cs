namespace BookingService.Business.Services.External;

public interface IPermissionService
{
    // Kiểm tra xem userId có quyền xem lịch của vehicleId không
    Task<bool> CanViewVehicleCalendarAsync(int userId, int vehicleId, string accessToken);
}