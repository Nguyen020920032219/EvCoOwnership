using BookingService.Business.Models;

namespace BookingService.Business.Services.Bookings;

public interface IBookingService
{
    Task<BookingResponseDto> CreateBookingAsync(int userId, BookingRequestDto request);
    Task<List<BookingResponseDto>> GetMyBookingsAsync(int userId);
}