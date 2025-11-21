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

    // Helper properties để code gọn hơn
    private int UserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
    private string? UserRole => User.FindFirst(ClaimTypes.Role)?.Value;

    [HttpPost("{voteId}/cast")]
    public async Task<IActionResult> CastVote(int voteId, [FromBody] CastVoteRequest request)
    {
        try
        {
            // Truyền thêm UserRole
            await _voteService.CastVoteAsync(UserId, UserRole, voteId, request);
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
            // Truyền thêm UserRole
            var list = await _voteService.GetVotesByGroupAsync(UserId, UserRole, groupId);
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
            // Truyền thêm UserRole
            var result = await _voteService.GetVoteResultAsync(UserId, UserRole, voteId);
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
            // Truyền thêm UserRole
            var result = await _voteService.CreateVoteAsync(UserId, UserRole, request);
            return Ok(ApiResult<VoteDetailDto>.Ok(result, "Tạo cuộc bỏ phiếu thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<object>.Fail(ex.Message));
        }
    }
}