using System.Security.Claims;
using ContractGroupService.Business.Models;
using ContractGroupService.Business.Services.Disputes;
using EvCoOwnership.Shared;
using EvCoOwnership.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractGroupService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DisputesController : ControllerBase
{
    private readonly IDisputeService _disputeService;

    public DisputesController(IDisputeService disputeService)
    {
        _disputeService = disputeService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDisputeRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _disputeService.CreateDisputeAsync(userId, request);
            return Ok(ApiResult<DisputeDetailDto>.Ok(result, "Gửi khiếu nại thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _disputeService.GetDisputeDetailAsync(userId, id);
            return Ok(ApiResult<DisputeDetailDto>.Ok(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }

    [HttpPost("{id}/message")]
    public async Task<IActionResult> AddMessage(int id, [FromBody] AddMessageRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _disputeService.AddMessageAsync(userId, id, request.Message);
            return Ok(ApiResult<string>.Ok("OK", "Gửi tin nhắn thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }
}