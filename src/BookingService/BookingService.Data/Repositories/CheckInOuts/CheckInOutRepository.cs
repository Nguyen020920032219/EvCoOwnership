using BookingService.Data.Configurations;
using BookingService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Data.Repositories.CheckInOuts;

public class CheckInOutRepository : BaseRepository<BookingDbContext, BookingCheckInOut, int>, ICheckInOutRepository
{
    public CheckInOutRepository(BookingDbContext context) : base(context)
    {
    }

    public async Task<List<BookingCheckInOut>> GetByBookingIdAsync(int bookingId)
    {
        return await DbSet()
            .Include(c => c.VehicleConditions)
            .Where(c => c.BookingId == bookingId)
            .OrderBy(c => c.DateTime)
            .ToListAsync();
    }
}