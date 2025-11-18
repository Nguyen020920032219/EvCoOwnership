using BookingService.Business.Models;

namespace BookingService.Business.Services.CheckInOuts;

public interface ICheckInOutService
{
    Task<CheckInOutResponseDto> PerformCheckInAsync(int userId, CheckInOutRequest request);
    Task<CheckInOutResponseDto> PerformCheckOutAsync(int userId, CheckInOutRequest request);
}