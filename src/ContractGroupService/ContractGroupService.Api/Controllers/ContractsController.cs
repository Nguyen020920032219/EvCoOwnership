using System.Security.Claims;
using ContractGroupService.Business.Models;
using ContractGroupService.Business.Services.Contracts;
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

    // Helper
    private int UserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    private string? UserRole => User.FindFirst(ClaimTypes.Role)?.Value;

    [HttpPost("generate")]
    public async Task<IActionResult> Generate([FromBody] GenerateContractRequest request)
    {
        try
        {
            // Truyền thêm UserRole
            var result = await _contractService.GenerateContractAsync(UserId, UserRole, request);
            return Ok(ApiResult<ContractDetailDto>.Ok(result, "Tạo/Cập nhật hợp đồng thành công"));
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
            // Truyền thêm UserRole
            var result = await _contractService.GetContractByGroupAsync(UserId, UserRole, groupId);
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
            // Hàm ký vẫn chỉ dùng UserId (logic cũ)
            await _contractService.SignContractAsync(UserId, contractId);
            return Ok(ApiResult<string>.Ok("OK", "Ký hợp đồng thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }
}