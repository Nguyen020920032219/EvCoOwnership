using BookingService.Business.Models;
using BookingService.Data.Entities;
using BookingService.Data.Repositories.Bookings;
using BookingService.Data.Repositories.CheckInOuts;

namespace BookingService.Business.Services.CheckInOuts;

public class CheckInOutService : ICheckInOutService
{
    private readonly IBookingRepository _bookingRepo;
    private readonly ICheckInOutRepository _checkInOutRepo;

    public CheckInOutService(ICheckInOutRepository checkInOutRepo, IBookingRepository bookingRepo)
    {
        _checkInOutRepo = checkInOutRepo;
        _bookingRepo = bookingRepo;
    }

    public async Task<CheckInOutResponseDto> PerformCheckInAsync(int userId, CheckInOutRequest request)
    {
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null) throw new Exception("Booking not found");

        // 1. Validate
        if (booking.UserId != userId) throw new Exception("Bạn không phải chủ booking này");
        if (booking.Status != 1) throw new Exception("Booking không ở trạng thái sẵn sàng (Upcoming)");

        // 2. Lưu Check-in
        var checkIn = new BookingCheckInOut
        {
            BookingId = request.BookingId,
            Type = 1, // 1 = CheckIn
            DateTime = DateTime.UtcNow
        };

        // Lưu điều kiện xe
        foreach (var cond in request.Conditions)
            checkIn.VehicleConditions.Add(new VehicleCondition
            {
                VehicleId = booking.VehicleId,
                Name = cond.Name,
                Detail = cond.Detail
            });

        await _checkInOutRepo.Add(checkIn);

        // 3. Update trạng thái Booking -> InProgress (2)
        booking.Status = 2;
        await _bookingRepo.Update(booking);

        // Save changes (Transaction ngầm định)

        return new CheckInOutResponseDto
            { CheckInOutId = checkIn.CheckInOutId, Type = "CheckIn", Time = checkIn.DateTime };
    }

    public async Task<CheckInOutResponseDto> PerformCheckOutAsync(int userId, CheckInOutRequest request)
    {
        var booking = await _bookingRepo.GetByIdAsync(request.BookingId);
        if (booking == null) throw new Exception("Booking not found");

        // 1. Validate
        if (booking.UserId != userId) throw new Exception("Bạn không phải chủ booking này");
        if (booking.Status != 2) throw new Exception("Booking chưa Check-in hoặc đã hoàn thành");

        // 2. Lưu Check-out
        var checkOut = new BookingCheckInOut
        {
            BookingId = request.BookingId,
            Type = 2, // 2 = CheckOut
            DateTime = DateTime.UtcNow
        };

        foreach (var cond in request.Conditions)
            checkOut.VehicleConditions.Add(new VehicleCondition
            {
                VehicleId = booking.VehicleId,
                Name = cond.Name,
                Detail = cond.Detail
            });

        await _checkInOutRepo.Add(checkOut);

        // 3. Update Booking -> Completed (3) & Lưu thông số vận hành
        booking.Status = 3;
        if (request.DistanceKm.HasValue) booking.DistanceKm = request.DistanceKm;
        if (request.EnergyKwh.HasValue) booking.EnergyKwh = request.EnergyKwh;

        await _bookingRepo.Update(booking);

        return new CheckInOutResponseDto
            { CheckInOutId = checkOut.CheckInOutId, Type = "CheckOut", Time = checkOut.DateTime };
    }
}