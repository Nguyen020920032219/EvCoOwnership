using AuthService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;

namespace AuthService.Data.Repositories.Profiles;

public interface IUserProfileRepository : IBaseRepository<UserProfile, int>
{
    Task<UserProfile?> GetByUserIdAsync(int userId);
}