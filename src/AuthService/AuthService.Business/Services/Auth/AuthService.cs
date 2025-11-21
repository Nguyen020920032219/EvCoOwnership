using AuthService.Business.Models;
using AuthService.Business.Services.JwtToken;
using AuthService.Data.Entities;
using AuthService.Data.Repositories.Profiles;
using AuthService.Data.Repositories.Users;
// Mới

// Mới

namespace AuthService.Business.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserProfileRepository _profileRepo;
    private readonly IUserRepository _userRepo;

    public AuthService(IUserRepository userRepo, IUserProfileRepository profileRepo,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _userRepo = userRepo;
        _profileRepo = profileRepo;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _userRepo.GetByPhoneNumberAsync(request.PhoneNumber);

        if (user == null) throw new Exception("Invalid phone or password.");

        if (user.PasswordHash != request.Password) throw new Exception("Invalid password.");

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
        var exists = await _userRepo.GetByPhoneNumberAsync(request.PhoneNumber);
        if (exists != null) throw new Exception("Phone number already exists.");

        var user = new AppUser
        {
            PhoneNumber = request.PhoneNumber,
            PasswordHash = request.Password,
            IsActive = request.ValidCitizenIdentification,
            RoleId = 3 // CoOwner
        };

        await _userRepo.Add(user);
        // await _userRepo.SaveChangesAsync();

        var profile = new UserProfile
        {
            UserId = user.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email
        };

        await _profileRepo.Add(profile);
        // await _profileRepo.SaveChangesAsync();
    }
}