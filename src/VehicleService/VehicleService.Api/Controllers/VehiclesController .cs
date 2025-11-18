using System.Security.Claims;
using EvCoOwnership.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleService.Business.Models;
using VehicleService.Business.Services;

namespace VehicleService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly IVehicleService _vehicleService;

    public VehiclesController(IVehicleService vehicleService)
    {
        _vehicleService = vehicleService;
    }

    [HttpGet("by-group/{groupId:int}")]
    public async Task<IActionResult> GetByGroup(int groupId)
    {
        // Nếu sau này muốn check user thuộc group thì lấy userId từ token ở đây
        // var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var vehicles = await _vehicleService.GetVehiclesByGroupAsync(groupId);

        return Ok(ApiResult<IReadOnlyList<VehicleDto>>.Ok(vehicles));
    }
}