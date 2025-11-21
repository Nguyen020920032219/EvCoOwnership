using ContractGroupService.Data.Configurations;
using ContractGroupService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace ContractGroupService.Data.Repositories.Disputes;

public class DisputeRepository : BaseRepository<ContractGroupDbContext, GroupDispute, int>, IDisputeRepository
{
    public DisputeRepository(ContractGroupDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<GroupDispute>> GetByGroupIdAsync(int groupId)
    {
        return await DbSet()
            .Where(d => d.CoOwnershipGroupId == groupId)
            .OrderByDescending(d => d.CreatedAt)
            .ToListAsync();
    }

    public async Task<GroupDispute?> GetDetailAsync(int disputeId)
    {
        return await DbSet()
            .Include(d => d.GroupDisputeMessages)
            .FirstOrDefaultAsync(d => d.GroupDisputeId == disputeId);
    }

    public async Task AddMessageAsync(GroupDisputeMessage message)
    {
        await _context.Set<GroupDisputeMessage>().AddAsync(message);
        await _context.SaveChangesAsync();
    }
}