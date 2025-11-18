using ContractGroupService.Data.Configurations;
using ContractGroupService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace ContractGroupService.Data.Repositories.Groups;

public class GroupRepository : BaseRepository<ContractGroupDbContext, CoOwnershipGroup, int>, IGroupRepository
{
    public GroupRepository(ContractGroupDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<CoOwnershipGroup>> GetGroupsByUserIdAsync(int userId)
    {
        // Join bảng Member để lọc
        return await _context.CoOwnershipMembers
            .Where(m => m.UserId == userId)
            .Select(m => m.CoOwnerGroup!)
            .ToListAsync();
    }

    public async Task<CoOwnershipGroup?> GetGroupDetailAsync(int groupId)
    {
        return await DbSet()
            .Include(g => g.CoOwnershipMembers)
            .Include(g => g.OwnershipShares)
            .FirstOrDefaultAsync(g => g.CoOwnerGroupId == groupId);
    }

    public async Task<bool> IsGroupAdminAsync(int groupId, int userId)
    {
        return await _context.CoOwnershipMembers
            .AnyAsync(m => m.CoOwnerGroupId == groupId && m.UserId == userId && m.IsGroupAdmin);
    }
}