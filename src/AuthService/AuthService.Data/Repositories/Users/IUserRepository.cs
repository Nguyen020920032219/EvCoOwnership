using AuthService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;

namespace AuthService.Data.Repositories.Users;

public interface IUserRepository : IBaseRepository<AppUser, int>
{
    Task<AppUser?> GetByPhoneNumberAsync(string phoneNumber);
    Task<AppUser?> GetUserWithProfileAsync(int userId);
    Task<List<AppUser>> GetAllWithProfileAsync();
}