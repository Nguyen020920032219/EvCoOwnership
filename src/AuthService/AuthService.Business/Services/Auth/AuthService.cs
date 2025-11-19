using AuthService.Business.Models;
using AuthService.Business.Services.JwtToken;
using AuthService.Data.Configurations;
using AuthService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Business.Services.Auth;

public class AuthService : IAuthService
{
    private readonly AuthDbContext _dbContext;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(AuthDbContext dbContext, IJwtTokenGenerator jwtTokenGenerator)
    {
        _dbContext = dbContext;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _dbContext.AppUsers
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);

        if (user == null) throw new Exception("Invalid phone or password.");

        if (user.PasswordHash != request.Password) throw new Exception("Invalid phone or password.");

        if (!user.IsActive) throw new Exception("User is not active.");

        var token = _jwtTokenGenerator.GenerateToken(user, user.Role.RoleName);

        return new LoginResponse
        {
            UserId = user.UserId,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.RoleName,
            AccessToken = token
        };
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var exists = await _dbContext.AppUsers
            .AnyAsync(u => u.PhoneNumber == request.PhoneNumber);

        if (exists) throw new Exception("Phone number already exists.");

        if (!request.ValidCitizenIdentification) throw new Exception("Invalid citizen identification.");

        var user = new AppUser
        {
            PhoneNumber = request.PhoneNumber,
            PasswordHash = request.Password,
            RoleId = 3
        };

        _dbContext.AppUsers.Add(user);

        var profile = new UserProfile
        {
            UserId = user.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };

        _dbContext.UserProfiles.Add(profile);
        await _dbContext.SaveChangesAsync();
    }
}