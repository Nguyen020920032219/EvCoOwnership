using BookingService.Business.Models;
using BookingService.Business.Services;
using BookingService.Data.Configurations;
using BookingService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace BookingService.Business;

public class BookingService : IBookingService
{
    private readonly BookingDbContext _db;

    public BookingService(BookingDbContext db)
    {
        _db = db;
    }

    public async Task<BookingResponseDto> CreateBookingAsync(int userId, BookingRequestDto request)
    {
        // Kiểm tra xe có tồn tại
        var vehicleExists = await _db.VehicleBookings.AnyAsync(v => v.VehicleId == request.VehicleId);
        if (!vehicleExists)
            throw new Exception("Vehicle not found");

        // Kiểm tra trùng lịch
        bool overlap = await _db.VehicleBookings.AnyAsync(b =>
            b.VehicleId == request.VehicleId &&
            b.StartDate < request.EndTime &&
            request.StartTime < b.EndDate
        );

        if (overlap)
            throw new Exception("Time slot already booked");

        var booking = new VehicleBooking()
        {
            VehicleId = request.VehicleId,
            UserId = userId,
            StartDate = request.StartTime,
            EndDate = request.EndTime
        };

        _db.VehicleBookings.Add(booking);
        await _db.SaveChangesAsync();

        return new BookingResponseDto
        {
            BookingId = booking.BookingId,
            VehicleId = booking.VehicleId,
            UserId = booking.UserId,
            StartTime = booking.StartDate,
            EndTime = booking.EndDate
        };
    }

    public async Task<List<BookingResponseDto>> GetMyBookingsAsync(int userId)
    {
        return await _db.VehicleBookings
            .Where(b => b.UserId == userId)
            .Select(b => new BookingResponseDto
            {
                BookingId = b.BookingId,
                VehicleId = b.VehicleId,
                UserId = b.UserId,
                StartTime = b.StartDate,
                EndTime = b.EndDate
            }).ToListAsync();
    }
}
