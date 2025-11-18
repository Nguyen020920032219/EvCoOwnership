using FinanceService.Business.Models;

namespace FinanceService.Business.Services.Funds;

public interface IFundService
{
    Task<FundDto> GetFundByGroupAsync(int groupId);
    Task<List<FundHistoryDto>> GetFundHistoryAsync(int groupId);
    Task DepositAsync(int userId, DepositRequest request);
}