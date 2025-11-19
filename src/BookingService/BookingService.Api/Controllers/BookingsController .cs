using System.Security.Claims;
using BookingService.Business.Models;
using BookingService.Business.Services.Bookings;
using BookingService.Business.Services.External;
using EvCoOwnership.Shared.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _service;
    private readonly IPermissionService _permissionService;

    public BookingsController(IBookingService service,  IPermissionService permissionService)
    {
        _service = service;
        _permissionService = permissionService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto request)
    {
        try
        {
            // Lấy UserId từ Token
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var userId = int.Parse(userIdStr);

            var result = await _service.CreateBookingAsync(userId, request);
            return Ok(ApiResult<BookingResponseDto>.Ok(result, "Đặt xe thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<BookingResponseDto>.Fail(ex.Message));
        }
    }

    [HttpGet("mine")]
    public async Task<IActionResult> MyBookings()
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

        var userId = int.Parse(userIdStr);
        var list = await _service.GetMyBookingsAsync(userId);

        return Ok(ApiResult<List<BookingResponseDto>>.Ok(list));
    }
    
    [HttpGet("vehicle/{vehicleId}/calendar")]
    public async Task<IActionResult> GetCalendar(int vehicleId, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
    {
        var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
        var userId = int.Parse(userIdStr);

        // Lấy Token hiện tại từ Request Header để chuyển tiếp (Forward)
        var accessToken = await HttpContext.GetTokenAsync("access_token");
        if (string.IsNullOrEmpty(accessToken)) return Unauthorized();

        // --- CHECK QUYỀN ---
        var canView = await _permissionService.CanViewVehicleCalendarAsync(userId, vehicleId, accessToken);
        if (!canView)
        {
            return StatusCode(403, ApiResult<string>.Fail("Bạn không thuộc nhóm sở hữu xe này, không thể xem lịch."));
        }
        // -------------------

        var startDate = from ?? DateTime.UtcNow;
        var endDate = to ?? DateTime.UtcNow.AddDays(30);

        try
        {
            var result = await _service.GetVehicleCalendarAsync(vehicleId, startDate, endDate);
            return Ok(ApiResult<List<VehicleCalendarDto>>.Ok(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }
}