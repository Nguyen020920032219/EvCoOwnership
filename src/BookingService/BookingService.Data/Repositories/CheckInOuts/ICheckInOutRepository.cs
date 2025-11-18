using BookingService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;

namespace BookingService.Data.Repositories.CheckInOuts;

public interface ICheckInOutRepository : IBaseRepository<BookingCheckInOut, int>
{
    // Lấy thông tin Check-in/out của một booking
    Task<List<BookingCheckInOut>> GetByBookingIdAsync(int bookingId);
}