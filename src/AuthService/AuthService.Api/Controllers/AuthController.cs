using AuthService.Business.Models;
using AuthService.Business.Services.Auth;
using EvCoOwnership.Shared.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        try
        {
            var result = await _authService.LoginAsync(request);
            return Ok(ApiResult<LoginResponse>.Ok(result, "Login success"));
        }
        catch (Exception ex)
        {
            var code = ErrorCodes.Auth_InvalidCredential;

            return BadRequest(ApiResult<LoginResponse>.Fail(ex.Message, code));
        }
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        try
        {
            await _authService.RegisterAsync(request);
            return Ok(ApiResult<string>.Ok("OK", "Register success"));
        }
        catch (Exception ex)
        {
            var errorCode = ex.Message.Contains("Phone number already exists", StringComparison.OrdinalIgnoreCase)
                ? ErrorCodes.Auth_PhoneAlreadyExists
                : ErrorCodes.BadRequest;

            return BadRequest(ApiResult<string>.Fail(ex.Message, errorCode));
        }
    }
}