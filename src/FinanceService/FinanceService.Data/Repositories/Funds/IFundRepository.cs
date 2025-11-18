using EvCoOwnership.Shared.BaseRepository;
using FinanceService.Data.Entities;

namespace FinanceService.Data.Repositories.Funds;

public interface IFundRepository : IBaseRepository<GroupFund, int>
{
    Task<GroupFund?> GetByGroupIdAsync(int groupId);
    Task AddHistoryAsync(GroupFundHistory history);
}