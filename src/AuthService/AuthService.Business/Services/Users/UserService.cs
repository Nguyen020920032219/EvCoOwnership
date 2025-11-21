using AuthService.Business.Models;
using AuthService.Data.Entities;
using AuthService.Data.Repositories.Profiles;
using AuthService.Data.Repositories.Users;

// Using mới

// Using mới

namespace AuthService.Business.Services.Users;

public class UserService : IUserService
{
    private readonly IUserProfileRepository _profileRepo;
    private readonly IUserRepository _userRepo;

    public UserService(IUserRepository userRepo, IUserProfileRepository profileRepo)
    {
        _userRepo = userRepo;
        _profileRepo = profileRepo;
    }

    public async Task<List<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepo.GetAllWithProfileAsync();

        return users.Select(u => new UserDto
        {
            UserId = u.UserId,
            PhoneNumber = u.PhoneNumber,
            RoleName = u.Role.RoleName,
            FirstName = u.UserProfile?.FirstName ?? "",
            LastName = u.UserProfile?.LastName ?? "",
            Email = u.UserProfile?.Email,
            IsActive = u.IsActive
        }).ToList();
    }

    public async Task<UserDto> GetUserByIdAsync(int userId)
    {
        var user = await _userRepo.GetUserWithProfileAsync(userId);
        if (user == null) throw new Exception("User not found");

        return new UserDto
        {
            UserId = user.UserId,
            PhoneNumber = user.PhoneNumber,
            RoleName = user.Role.RoleName,
            FirstName = user.UserProfile?.FirstName ?? "",
            LastName = user.UserProfile?.LastName ?? "",
            Email = user.UserProfile?.Email,
            IsActive = user.IsActive
        };
    }

    public async Task<UserDto> CreateUserAsync(CreateUserByAdminRequest request)
    {
        // Check trùng phone
        var existing = await _userRepo.GetByPhoneNumberAsync(request.PhoneNumber);
        if (existing != null) throw new Exception("Phone number already exists");

        var user = new AppUser
        {
            PhoneNumber = request.PhoneNumber,
            PasswordHash = request.Password,
            RoleId = request.RoleId
        };

        await _userRepo.Add(user);
        // Save để có UserId
        // await _userRepo.SaveChangesAsync(); 

        var profile = new UserProfile
        {
            UserId = user.UserId,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = null
        };

        await _profileRepo.Add(profile);
        // await _profileRepo.SaveChangesAsync();

        return await GetUserByIdAsync(user.UserId);
    }

    public async Task UpdateUserAsync(int userId, UpdateUserByAdminRequest request)
    {
        var user = await _userRepo.GetUserWithProfileAsync(userId);
        if (user == null) throw new Exception("User not found");

        // Update Role
        if (request.RoleId.HasValue)
        {
            user.RoleId = request.RoleId.Value;
            await _userRepo.Update(user);
        }

        // Update Profile
        if (user.UserProfile == null)
        {
            user.UserProfile = new UserProfile { UserId = userId };
            await _profileRepo.Add(user.UserProfile);
        }

        user.UserProfile.FirstName = request.FirstName;
        user.UserProfile.LastName = request.LastName;
        user.UserProfile.Email = request.Email;

        // Save thông qua Repo tương ứng (hoặc UnitOfWork)
        // Ở đây dùng repo profile update entity profile
        await _profileRepo.Update(user.UserProfile);

        // await _userRepo.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _userRepo.GetByIdAsync(userId);
        if (user == null) throw new Exception("User not found");

        var profile = await _profileRepo.GetByUserIdAsync(userId);
        if (profile != null) await _profileRepo.Delete(profile);

        await _userRepo.Delete(user);
        // await _userRepo.SaveChangesAsync();
    }
}