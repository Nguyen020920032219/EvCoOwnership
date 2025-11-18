using ContractGroupService.Data.Configurations;
using ContractGroupService.Data.Entities;
using EvCoOwnership.Shared.BaseRepository;
using Microsoft.EntityFrameworkCore;

namespace ContractGroupService.Data.Repositories.Votes;

public class VoteRepository : BaseRepository<ContractGroupDbContext, GroupVote, int>, IVoteRepository
{
    public VoteRepository(ContractGroupDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<GroupVote>> GetVotesByGroupIdAsync(int groupId)
    {
        return await DbSet()
            .Where(v => v.CoOwnerGroupId == groupId)
            .OrderByDescending(v => v.CreatedAt)
            .ToListAsync();
    }

    public async Task<GroupVote?> GetVoteDetailAsync(int voteId)
    {
        return await DbSet()
            .Include(v => v.VoteChoices) // Load danh sách người đã vote
            .FirstOrDefaultAsync(v => v.VoteId == voteId);
    }

    public async Task<bool> HasUserVotedAsync(int voteId, int userId)
    {
        return await _context.VoteChoices
            .AnyAsync(vc => vc.VoteId == voteId && vc.UserId == userId);
    }

    public async Task AddVoteChoiceAsync(VoteChoice choice)
    {
        await _context.VoteChoices.AddAsync(choice);
        // Lưu ý: SaveChanges sẽ được gọi ở Service hoặc UnitOfWork
    }
}