using AuthService.Data.Configurations;
using AuthService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data.Repositories.Profiles;

public class UserProfileRepository : BaseRepository<AuthDbContext, UserProfile, int>, IUserProfileRepository
{
    public UserProfileRepository(AuthDbContext context) : base(context)
    {
    }

    public async Task<UserProfile?> GetByUserIdAsync(int userId)
    {
        return await DbSet().FirstOrDefaultAsync(p => p.UserId == userId);
    }
}