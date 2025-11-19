using AuthService.Business.Models;

namespace AuthService.Business.Services.Users;

public interface IUserService
{
    Task<List<UserDto>> GetAllUsersAsync();
    Task<UserDto> GetUserByIdAsync(int userId);
    Task<UserDto> CreateUserAsync(CreateUserByAdminRequest request);
    Task UpdateUserAsync(int userId, UpdateUserByAdminRequest request);
    Task DeleteUserAsync(int userId);
}