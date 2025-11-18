using BookingService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;

namespace BookingService.Data.Repositories.Bookings;

public interface IBookingRepository : IBaseRepository<VehicleBooking, int>
{
    // Hàm kiểm tra xem xe có bị trùng lịch không
    Task<bool> HasOverlapAsync(int vehicleId, DateTime start, DateTime end);

    // Lấy danh sách booking của user
    Task<IReadOnlyList<VehicleBooking>> GetByUserIdAsync(int userId);
}