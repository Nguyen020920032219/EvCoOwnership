using EvCoOwnership.Shared.BaseRepository;
using FinanceService.Data.Configurations;
using FinanceService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Data.Repositories.Funds;

public class FundRepository : BaseRepository<FinanceDbContext, GroupFund, int>, IFundRepository
{
    public FundRepository(FinanceDbContext context) : base(context)
    {
    }

    public async Task<GroupFund?> GetByGroupIdAsync(int groupId)
    {
        return await DbSet()
            .Include(f => f.GroupFundHistories)
            .FirstOrDefaultAsync(f => f.CoOwnerGroupId == groupId);
    }

    public async Task AddHistoryAsync(GroupFundHistory history)
    {
        await _context.Set<GroupFundHistory>().AddAsync(history);
    }
}