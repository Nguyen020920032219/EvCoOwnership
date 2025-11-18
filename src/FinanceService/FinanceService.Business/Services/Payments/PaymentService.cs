using FinanceService.Business.Models;
using FinanceService.Data.Entities;
using FinanceService.Data.Repositories.Funds;
using FinanceService.Data.Repositories.Payments;

namespace FinanceService.Business.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IPaymentRepository _paymentRepo;
    private readonly IFundRepository _fundRepo; // Cần để cộng tiền vào quỹ

    public PaymentService(IPaymentRepository paymentRepo, IFundRepository fundRepo)
    {
        _paymentRepo = paymentRepo;
        _fundRepo = fundRepo;
    }

    public async Task<PaymentTransactionDto> InitiatePaymentAsync(int userId, InitiatePaymentRequest request)
    {
        var trans = new PaymentTransaction
        {
            UserId = userId,
            CoOwnerGroupId = request.CoOwnerGroupId,
            SharedExpenseId = request.SharedExpenseId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod,
            Status = 0, // Pending
            CreatedAt = DateTime.UtcNow,
            ProviderTransactionId = Guid.NewGuid().ToString() // Fake mã giao dịch
        };

        await _paymentRepo.Add(trans);

        return MapToDto(trans);
    }

    public async Task ConfirmPaymentSuccessAsync(int transactionId)
    {
        var trans = await _paymentRepo.GetByIdAsync(transactionId);
        if (trans == null) throw new Exception("Transaction not found");
        if (trans.Status == 1) return; // Đã success rồi thì thôi

        // 1. Update trạng thái giao dịch
        trans.Status = 1; // Success
        trans.UpdatedAt = DateTime.UtcNow;
        await _paymentRepo.Update(trans);

        // 2. Cộng tiền vào Quỹ chung (Logic quan trọng)
        // Bất kể là trả tiền nợ (Expense) hay nạp quỹ (Deposit), tiền đều chảy về Quỹ của nhóm
        var fund = await _fundRepo.GetByGroupIdAsync(trans.CoOwnerGroupId);
        if (fund == null)
        {
            // Nếu chưa có quỹ thì tạo mới (Phòng hờ)
            fund = new GroupFund { CoOwnerGroupId = trans.CoOwnerGroupId, Amount = 0 };
            await _fundRepo.Add(fund);
        }

        fund.Amount += trans.Amount;
        await _fundRepo.Update(fund);

        // 3. Ghi log lịch sử quỹ
        var reason = trans.SharedExpenseId.HasValue 
            ? $"Thanh toán chi phí #{trans.SharedExpenseId}" 
            : "Nạp quỹ trực tiếp";

        await _fundRepo.AddHistoryAsync(new GroupFundHistory
        {
            FundId = fund.FundId,
            ChangeAmount = trans.Amount,
            Reason = reason,
            ChangedByUserId = trans.UserId,
            ChangedAt = DateTime.UtcNow
        });

        await _fundRepo.DbContext().SaveChangesAsync();
    }

    public async Task<List<PaymentTransactionDto>> GetMyHistoryAsync(int userId, int groupId)
    {
        var list = await _paymentRepo.GetByUserIdAsync(userId, groupId);
        return list.Select(MapToDto).ToList();
    }

    private static PaymentTransactionDto MapToDto(PaymentTransaction t)
    {
        return new PaymentTransactionDto
        {
            TransactionId = t.PaymentTransactionId,
            Amount = t.Amount,
            Status = t.Status == 0 ? "Pending" : t.Status == 1 ? "Success" : "Failed",
            Type = t.SharedExpenseId.HasValue ? "Expense" : "Deposit",
            CreatedAt = t.CreatedAt
        };
    }
}