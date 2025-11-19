using System.Security.Claims;
using AuthService.Business.Models;
using AuthService.Business.Services.Profiles;
using EvCoOwnership.Shared;
using EvCoOwnership.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/profiles")]
[Authorize] // Yêu cầu đăng nhập
public class UserProfilesController : ControllerBase
{
    private readonly IUserProfileService _profileService;

    public UserProfilesController(IUserProfileService profileService)
    {
        _profileService = profileService;
    }

    // GET: /api/profiles/me
    [HttpGet("me")]
    public async Task<IActionResult> GetMyProfile()
    {
        try
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var result = await _profileService.GetMyProfileAsync(int.Parse(userIdStr));
            return Ok(ApiResult<MyProfileDto>.Ok(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }

    // PUT: /api/profiles/me
    [HttpPut("me")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateMyProfileRequest request)
    {
        try
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

            var result = await _profileService.UpdateMyProfileAsync(int.Parse(userIdStr), request);
            return Ok(ApiResult<MyProfileDto>.Ok(result, "Cập nhật hồ sơ thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }
}