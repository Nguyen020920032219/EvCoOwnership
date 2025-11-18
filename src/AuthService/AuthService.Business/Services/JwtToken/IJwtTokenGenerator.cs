using AuthService.Data.Entities;

namespace AuthService.Business.Services.JwtToken;

public interface IJwtTokenGenerator
{
    string GenerateToken(AppUser user, string roleName);
}