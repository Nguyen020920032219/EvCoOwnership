using FinanceService.Business.Models;
using FinanceService.Data.Entities;
using FinanceService.Data.Repositories.Funds;

namespace FinanceService.Business.Services.Funds;

public class FundService : IFundService
{
    private readonly IFundRepository _fundRepo;

    public FundService(IFundRepository fundRepo)
    {
        _fundRepo = fundRepo;
    }

    public async Task<FundDto> GetFundByGroupAsync(int groupId)
    {
        var fund = await _fundRepo.GetByGroupIdAsync(groupId);
        if (fund == null) return new FundDto { CoOwnerGroupId = groupId, Amount = 0 };

        return new FundDto
        {
            FundId = fund.FundId,
            CoOwnerGroupId = fund.CoOwnerGroupId,
            Amount = fund.Amount
        };
    }

    public async Task<List<FundHistoryDto>> GetFundHistoryAsync(int groupId)
    {
        var fund = await _fundRepo.GetByGroupIdAsync(groupId);
        if (fund == null || fund.GroupFundHistories == null) return new List<FundHistoryDto>();

        return fund.GroupFundHistories.OrderByDescending(h => h.ChangedAt).Select(h => new FundHistoryDto
        {
            ChangeAmount = h.ChangeAmount,
            Reason = h.Reason,
            CreatedAt = h.ChangedAt,
            ChangedByUserId = h.ChangedByUserId
        }).ToList();
    }

    public async Task DepositAsync(int userId, DepositRequest request)
    {
        if (request.Amount <= 0) throw new Exception("Số tiền nạp phải lớn hơn 0");

        // 1. Lấy quỹ hiện tại hoặc tạo mới nếu chưa có
        var fund = await _fundRepo.GetByGroupIdAsync(request.CoOwnerGroupId);
        if (fund == null)
        {
            fund = new GroupFund
            {
                CoOwnerGroupId = request.CoOwnerGroupId,
                Amount = 0
            };
            await _fundRepo.Add(fund);
            // Cần save để có FundId trước khi thêm History
        }

        // 2. Cộng tiền
        fund.Amount += request.Amount;
        await _fundRepo.Update(fund);

        // 3. Ghi lịch sử
        var history = new GroupFundHistory
        {
            FundId = fund.FundId,
            ChangeAmount = request.Amount,
            Reason = request.Reason,
            ChangedByUserId = userId,
            ChangedAt = DateTime.UtcNow
        };

        await _fundRepo.AddHistoryAsync(history);
        await _fundRepo.DbContext().SaveChangesAsync();
    }
}