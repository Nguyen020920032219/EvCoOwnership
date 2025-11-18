using EvCoOwnership.Shared.BaseRepository;
using FinanceService.Data.Configurations;
using FinanceService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Data.Repositories.Expenses;

public class ExpenseRepository : BaseRepository<FinanceDbContext, SharedExpense, int>, IExpenseRepository
{
    public ExpenseRepository(FinanceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<SharedExpense>> GetByGroupIdAsync(int groupId)
    {
        return await DbSet()
            .Include(e => e.SharedExpenseShares)
            .Where(e => e.CoOwnerGroupId == groupId)
            .OrderByDescending(e => e.ExpenseDate)
            .ToListAsync();
    }
}