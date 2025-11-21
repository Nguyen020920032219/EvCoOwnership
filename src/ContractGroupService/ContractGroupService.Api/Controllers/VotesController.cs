using System.Security.Claims;
using ContractGroupService.Business.Models;
using ContractGroupService.Business.Services.Votes;
using EvCoOwnership.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractGroupService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class VotesController : ControllerBase
{
    private readonly IVoteService _voteService;

    public VotesController(IVoteService voteService)
    {
        _voteService = voteService;
    }

    [HttpPost("{voteId}/cast")]
    public async Task<IActionResult> CastVote(int voteId, [FromBody] CastVoteRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await _voteService.CastVoteAsync(userId, voteId, request);
            return Ok(ApiResult<string>.Ok("OK", "Bỏ phiếu thành công"));
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
            var list = await _voteService.GetVotesByGroupAsync(userId, groupId);
            return Ok(ApiResult<List<VoteDetailDto>>.Ok(list));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<object>.Fail(ex.Message));
        }
    }

    [HttpGet("{voteId}")]
    public async Task<IActionResult> GetDetail(int voteId)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _voteService.GetVoteResultAsync(userId, voteId);
            return Ok(ApiResult<VoteDetailDto>.Ok(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<object>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateVote([FromBody] CreateVoteRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Lấy Role từ Claims
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value ?? "CoOwner";

            // Truyền Role xuống Service
            var result = await _voteService.CreateVoteAsync(userId, userRole, request);

            return Ok(ApiResult<VoteDetailDto>.Ok(result, "Tạo cuộc bỏ phiếu thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<object>.Fail(ex.Message));
        }
    }
}