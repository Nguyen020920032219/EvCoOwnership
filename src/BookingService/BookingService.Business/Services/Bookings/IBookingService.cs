using BookingService.Business.Models;
using BookingService.Data.Entities;

namespace BookingService.Business.Services.Bookings;

public interface IBookingService
{
    Task<BookingResponseDto> CreateBookingAsync(int userId, BookingRequestDto request);
    Task<List<BookingResponseDto>> GetMyBookingsAsync(int userId);
    Task<List<VehicleCalendarDto>> GetVehicleCalendarAsync(int vehicleId, DateTime from, DateTime to);}