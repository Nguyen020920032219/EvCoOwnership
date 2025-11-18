using System.Security.Claims;
using ContractGroupService.Business.Models;
using ContractGroupService.Business.Services.Contracts;
using EvCoOwnership.Shared;
using EvCoOwnership.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractGroupService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ContractsController : ControllerBase
{
    private readonly IContractService _contractService;

    public ContractsController(IContractService contractService)
    {
        _contractService = contractService;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GenerateContractRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _contractService.GenerateContractAsync(userId, request);
            return Ok(ApiResult<ContractDetailDto>.Ok(result, "Tạo hợp đồng thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }

    [HttpGet("group/{groupId}")]
    public async Task<IActionResult> GetByGroup(int groupId)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _contractService.GetContractByGroupAsync(userId, groupId);
            return Ok(ApiResult<ContractDetailDto>.Ok(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }

    [HttpPost("{contractId}/sign")]
    public async Task<IActionResult> Sign(int contractId)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _contractService.SignContractAsync(userId, contractId);
            return Ok(ApiResult<string>.Ok("OK", "Ký hợp đồng thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }
}