using EvCoOwnership.Shared.Models;
using Microsoft.AspNetCore.Mvc;
using VehicleService.Business.Models;
using VehicleService.Business.Services.Vehicles;

namespace VehicleService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        var data = await _vehicleService.GetVehiclesByGroupAsync(groupId);
        return Ok(ApiResult<IReadOnlyList<VehicleDto>>.Ok(data));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateVehicleRequest request)
    {
        try
        {
            var result = await _vehicleService.CreateVehicleAsync(request);
            return Ok(ApiResult<VehicleDto>.Ok(result, "Thêm xe mới thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<VehicleDto>.Fail(ex.Message));
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _vehicleService.GetVehicles();
        return Ok(ApiResult<List<VehicleDto>>.Ok(result));
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _vehicleService.GetVehicleByIdAsync(id);
        if (result == null) return NotFound(ApiResult<string>.Fail("Vehicle not found"));
        
        return Ok(ApiResult<VehicleDto>.Ok(result));
    }
}