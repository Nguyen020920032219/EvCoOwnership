using System.Security.Claims;
using BookingService.Business.Models;
using BookingService.Business.Services.CheckInOuts;
using EvCoOwnership.Shared;
using EvCoOwnership.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookingService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CheckInOutController : ControllerBase
{
    private readonly ICheckInOutService _service;

    public CheckInOutController(ICheckInOutService service)
    {
        _service = service;
    }

    [HttpPost("check-in")]
    public async Task<IActionResult> CheckIn([FromBody] CheckInOutRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _service.PerformCheckInAsync(userId, request);
            return Ok(ApiResult<CheckInOutResponseDto>.Ok(result, "Nhận xe thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }

    [HttpPost("check-out")]
    public async Task<IActionResult> CheckOut([FromBody] CheckInOutRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _service.PerformCheckOutAsync(userId, request);
            return Ok(ApiResult<CheckInOutResponseDto>.Ok(result, "Trả xe thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }
}