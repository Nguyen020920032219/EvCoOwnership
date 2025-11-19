using AuthService.Business.Models;
using AuthService.Business.Services.Users;
using EvCoOwnership.Shared;
using EvCoOwnership.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/admin/users")]
[Authorize(Roles = "Admin")] // Chỉ Admin mới được gọi
public class AdminUsersController : ControllerBase
{
    private readonly IUserService _userService;

    public AdminUsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(ApiResult<List<UserDto>>.Ok(users));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            return Ok(ApiResult<UserDto>.Ok(user));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResult<string>.Fail(ex.Message));
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserByAdminRequest request)
    {
        try
        {
            var user = await _userService.CreateUserAsync(request);
            return Ok(ApiResult<UserDto>.Ok(user, "Tạo user thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUserByAdminRequest request)
    {
        try
        {
            await _userService.UpdateUserAsync(id, request);
            return Ok(ApiResult<string>.Ok("OK", "Cập nhật thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return Ok(ApiResult<string>.Ok("OK", "Xóa user thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }
}