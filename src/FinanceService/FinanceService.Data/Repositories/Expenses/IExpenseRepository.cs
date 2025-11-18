using EvCoOwnership.Shared.BaseRepository;
using FinanceService.Data.Entities;

namespace FinanceService.Data.Repositories.Expenses;

public interface IExpenseRepository : IBaseRepository<SharedExpense, int>
{
    Task<IReadOnlyList<SharedExpense>> GetByGroupIdAsync(int groupId);
}