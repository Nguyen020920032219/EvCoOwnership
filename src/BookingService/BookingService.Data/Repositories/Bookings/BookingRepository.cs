using BookingService.Data.Configurations;
using BookingService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Data.Repositories.Bookings;

public class BookingRepository : BaseRepository<BookingDbContext, VehicleBooking, int>, IBookingRepository
{
    public BookingRepository(BookingDbContext context) : base(context)
    {
    }

    public async Task<bool> HasOverlapAsync(int vehicleId, DateTime start, DateTime end)
    {
        // Logic: Một booking mới (start, end) sẽ trùng nếu tồn tại booking cũ (b) mà:
        // b.Start < new.End AND b.End > new.Start
        // Và trạng thái booking cũ phải là Active (Upcoming/InProgress) - Giả sử Status 1, 2 là active
        return await DbSet().AnyAsync(b =>
            b.VehicleId == vehicleId &&
            (b.Status == 1 || b.Status == 2) &&
            b.StartDate < end &&
            b.EndDate > start
        );
    }

    public async Task<IReadOnlyList<VehicleBooking>> GetByUserIdAsync(int userId)
    {
        return await Where(b => b.UserId == userId)
            .OrderByDescending(b => b.StartDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<VehicleBooking>> GetBookingsByVehicleAsync(int vehicleId, DateTime from,
        DateTime to)
    {
        // Lấy các booking CÓ thời gian đè lên khoảng [from, to]
        // Logic overlap: (BookingStart < RangeEnd) AND (BookingEnd > RangeStart)
        // Status != 4 (Canceled)
        return await DbSet()
            .Where(b => b.VehicleId == vehicleId
                        && b.Status != 4
                        && b.StartDate < to
                        && b.EndDate > from)
            .OrderBy(b => b.StartDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<VehicleBooking>> GetByGroupIdAsync(int groupId)
    {
        return await DbSet()
            .Where(b => b.CoOwnerGroupId == groupId)
            .OrderByDescending(b => b.StartDate) // Mới nhất lên đầu
            .ToListAsync();
    }
}