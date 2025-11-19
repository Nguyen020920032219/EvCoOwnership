using AuthService.Business.Models;

namespace AuthService.Business.Services.Profiles;

public interface IUserProfileService
{
    // Lấy hồ sơ của chính mình
    Task<MyProfileDto> GetMyProfileAsync(int userId);
    
    // Cập nhật hồ sơ của chính mình
    Task<MyProfileDto> UpdateMyProfileAsync(int userId, UpdateMyProfileRequest request);
}