using AuthService.Data.Configurations;
using AuthService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data.Repositories.Users;

public class UserRepository : BaseRepository<AuthDbContext, AppUser, int>, IUserRepository
{
    public UserRepository(AuthDbContext context) : base(context)
    {
    }

    public async Task<AppUser?> GetByPhoneNumberAsync(string phoneNumber)
    {
        return await DbSet()
            .Include(u => u.Role)
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber);
    }

    public async Task<AppUser?> GetUserWithProfileAsync(int userId)
    {
        return await DbSet()
            .Include(u => u.Role)
            .Include(u => u.UserProfile)
            .FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<List<AppUser>> GetAllWithProfileAsync()
    {
        return await DbSet()
            .Include(u => u.Role)
            .Include(u => u.UserProfile)
            .ToListAsync();
    }
}