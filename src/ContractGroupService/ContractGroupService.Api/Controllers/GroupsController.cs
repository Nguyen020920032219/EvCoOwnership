using System.Security.Claims;
using ContractGroupService.Business.Models;
using ContractGroupService.Business.Services.Group;
using EvCoOwnership.Shared;
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

    [HttpGet("my")]
    public async Task<IActionResult> GetMyGroups()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(ApiResult<IReadOnlyList<CoOwnerGroupDto>>.Fail(
                "Invalid user id in token.",
                ErrorCodes.Unauthorized
            ));
        }

        var groups = await _groupService.GetGroupsByUserAsync(userId);

        if (groups.Count == 0)
        {
            return Ok(ApiResult<IReadOnlyList<CoOwnerGroupDto>>.Ok(groups, "User has no co-ownership groups."));
        }

        return Ok(ApiResult<IReadOnlyList<CoOwnerGroupDto>>.Ok(groups));
    }
}