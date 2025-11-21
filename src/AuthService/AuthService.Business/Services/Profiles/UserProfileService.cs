using AuthService.Business.Models;
using AuthService.Data.Entities;
using AuthService.Data.Repositories.Profiles;
using AuthService.Data.Repositories.Users;

namespace AuthService.Business.Services.Profiles;

public class UserProfileService : IUserProfileService
{
    private readonly IUserProfileRepository _profileRepo;
    private readonly IUserRepository _userRepo;

    public UserProfileService(IUserRepository userRepo, IUserProfileRepository profileRepo)
    {
        _userRepo = userRepo;
        _profileRepo = profileRepo;
    }

    public async Task<MyProfileDto> GetMyProfileAsync(int userId)
    {
        var user = await _userRepo.GetUserWithProfileAsync(userId);
        if (user == null) throw new Exception("User not found");

        return MapToDto(user);
    }

    public async Task<MyProfileDto> UpdateMyProfileAsync(int userId, UpdateMyProfileRequest request)
    {
        var user = await _userRepo.GetUserWithProfileAsync(userId);
        if (user == null) throw new Exception("User not found");

        if (user.UserProfile == null)
        {
            user.UserProfile = new UserProfile { UserId = userId };
            await _profileRepo.Add(user.UserProfile);
        }

        user.UserProfile.FirstName = request.FirstName;
        user.UserProfile.LastName = request.LastName;
        user.UserProfile.Email = request.Email;
        user.UserProfile.Gender = request.Gender;
        user.UserProfile.DateOfBirth = request.DateOfBirth;
        user.UserProfile.Address = request.Address;

        await _profileRepo.Update(user.UserProfile);
        // await _profileRepo.SaveChangesAsync();

        return MapToDto(user);
    }

    private static MyProfileDto MapToDto(AppUser user)
    {
        var p = user.UserProfile;
        return new MyProfileDto
        {
            UserId = user.UserId,
            PhoneNumber = user.PhoneNumber,
            Role = user.Role.RoleName,
            FirstName = p?.FirstName ?? "",
            LastName = p?.LastName ?? "",
            Email = p?.Email,
            Gender = p?.Gender,
            DateOfBirth = p?.DateOfBirth,
            Address = p?.Address
        };
    }
}