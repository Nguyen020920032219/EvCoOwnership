using System.Security.Claims;
using ContractGroupService.Business.Models;
using ContractGroupService.Business.Services.Groups;
using EvCoOwnership.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ContractGroupService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupsController(IGroupService groupService)
    {
        _groupService = groupService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGroup([FromBody] CreateGroupRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _groupService.CreateGroupAsync(userId, request);
            return Ok(ApiResult<GroupDetailDto>.Ok(result, "Tạo nhóm thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<GroupDetailDto>.Fail(ex.Message));
        }
    }

    [HttpGet("my")]
    public async Task<IActionResult> GetMyGroups()
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var list = await _groupService.GetMyGroupsAsync(userId);
        return Ok(ApiResult<List<CoOwnerGroupDto>>.Ok(list));
    }

    [HttpGet("{groupId}")]
    public async Task<IActionResult> GetGroupDetail(int groupId)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var detail = await _groupService.GetGroupDetailAsync(groupId, userId);
            return Ok(ApiResult<GroupDetailDto>.Ok(detail));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }
}