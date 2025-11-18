using System.Security.Claims;
using BookingService.Business.Models;
using BookingService.Business.Services.Bookings;
using EvCoOwnership.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _service;

    public BookingsController(IBookingService service)
    {
        _service = service;
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
}