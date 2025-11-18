using System.Security.Claims;
using EvCoOwnership.Shared.Models;
using FinanceService.Business.Models;
using FinanceService.Business.Services.Funds;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FundsController : ControllerBase
{
    private readonly IFundService _fundService;

    public FundsController(IFundService fundService)
    {
        _fundService = fundService;
    }

    [HttpGet("group/{groupId}")]
    public async Task<IActionResult> GetBalance(int groupId)
    {
        var result = await _fundService.GetFundByGroupAsync(groupId);
        return Ok(ApiResult<FundDto>.Ok(result));
    }

    [HttpGet("group/{groupId}/history")]
    public async Task<IActionResult> GetHistory(int groupId)
    {
        var result = await _fundService.GetFundHistoryAsync(groupId);
        return Ok(ApiResult<List<FundHistoryDto>>.Ok(result));
    }

    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromBody] DepositRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _fundService.DepositAsync(userId, request);
            return Ok(ApiResult<string>.Ok("OK", "Nạp quỹ thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }
}