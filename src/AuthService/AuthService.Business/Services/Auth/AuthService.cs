using AuthService.Business.Models;
using AuthService.Business.Services.JwtToken;
using AuthService.Data.Configurations;
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
        // Tìm user theo phone
        var user = await _dbContext.AppUsers
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.PhoneNumber == request.PhoneNumber);

        if (user == null)
        {
            throw new Exception("Invalid phone or password.");
        }

        // TODO: sau này dùng hash, giờ tạm plain text cho dễ test:
        // if (user.PasswordHash != request.Password)
        // {
        //     throw new Exception("Invalid phone or password.");
        // }

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
        // Check trùng phone
        var exists = await _dbContext.AppUsers
            .AnyAsync(u => u.PhoneNumber == request.PhoneNumber);

        if (exists)
        {
            throw new Exception("Phone number already exists.");
        }

        // TODO: hash password, demo thì lưu plain
        var user = new Data.Entities.AppUser
        {
            PhoneNumber = request.PhoneNumber,
            PasswordHash = request.Password,
            RoleId = 3 // CoOwner
        };

        _dbContext.AppUsers.Add(user);
        await _dbContext.SaveChangesAsync();

        // Tạo profile
        var profile = new Data.Entities.UserProfile
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
