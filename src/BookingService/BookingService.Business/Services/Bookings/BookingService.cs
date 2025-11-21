using BookingService.Business.Models;
using BookingService.Data.Entities;
using BookingService.Data.Repositories.Bookings;

namespace BookingService.Business.Services.Bookings;

public class BookingService : IBookingService
{
    private readonly IBookingRepository _bookingRepo;

    public BookingService(IBookingRepository bookingRepo)
    {
        _bookingRepo = bookingRepo;
    }

    public async Task<BookingResponseDto> CreateBookingAsync(int userId, BookingRequestDto request)
    {
        // 1. Validate logic thời gian
        if (request.StartTime >= request.EndTime) throw new Exception("Thời gian kết thúc phải sau thời gian bắt đầu.");

        if (request.StartTime < DateTime.Now) throw new Exception("Không thể đặt lịch trong quá khứ.");

        // 2. Check trùng lịch (Business Rule quan trọng)
        var isOverlap = await _bookingRepo.HasOverlapAsync(request.VehicleId, request.StartTime, request.EndTime);
        if (isOverlap) throw new Exception("Xe đã có người đặt trong khoảng thời gian này.");

        // 3. Tạo Entity
        var booking = new VehicleBooking
        {
            UserId = userId,
            VehicleId = request.VehicleId,
            StartDate = request.StartTime,
            EndDate = request.EndTime,
            Status = 1, // 1 = Upcoming (như trong DB Script)
            CoOwnerGroupId = 0 // TODO: Cần lấy GroupId từ VehicleService hoặc Client gửi lên
        };

        // 4. Lưu xuống DB
        await _bookingRepo.Add(booking);
        // await _bookingRepo.SaveChangesAsync(); // Nếu BaseRepo chưa save

        // 5. Map sang DTO trả về
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
        var bookings = await _bookingRepo.GetByUserIdAsync(userId);

        return bookings.Select(b => new BookingResponseDto
        {
            BookingId = b.BookingId,
            VehicleId = b.VehicleId,
            UserId = b.UserId,
            StartTime = b.StartDate,
            EndTime = b.EndDate
        }).ToList();
    }

    public async Task<List<VehicleCalendarDto>> GetVehicleCalendarAsync(int vehicleId, DateTime from, DateTime to)
    {
        var bookings = await _bookingRepo.GetBookingsByVehicleAsync(vehicleId, from, to);

        return bookings.Select(b => new VehicleCalendarDto
        {
            BookingId = b.BookingId,
            StartTime = b.StartDate,
            EndTime = b.EndDate,
            Status = b.Status switch
            {
                1 => "Upcoming",
                2 => "InProgress",
                3 => "Completed",
                _ => "Unknown"
            }
        }).ToList();
    }

    public async Task<List<BookingResponseDto>> GetGroupBookingsAsync(int userId, int groupId, string accessToken)
    {
        // Lưu ý: Việc check user có thuộc group không sẽ làm ở Controller cho gọn code Service này
        // hoặc bạn có thể inject IPermissionService vào đây.

        var bookings = await _bookingRepo.GetByGroupIdAsync(groupId);

        return bookings.Select(b => new BookingResponseDto
        {
            BookingId = b.BookingId,
            VehicleId = b.VehicleId,
            UserId = b.UserId,
            StartTime = b.StartDate,
            EndTime = b.EndDate
            // Nếu muốn hiện tên người đặt, cần gọi AuthService để lấy Profile,
            // nhưng để đơn giản ở MVP thì trả về UserId là đủ.
        }).ToList();
    }
}