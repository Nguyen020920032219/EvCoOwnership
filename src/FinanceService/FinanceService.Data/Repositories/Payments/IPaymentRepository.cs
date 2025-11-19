using EvCoOwnership.Shared.BaseRepository;
using FinanceService.Data.Entities;

namespace FinanceService.Data.Repositories.Payments;

public interface IPaymentRepository : IBaseRepository<PaymentTransaction, int>
{
    // Lấy lịch sử giao dịch của user trong 1 nhóm
    Task<IReadOnlyList<PaymentTransaction>> GetByUserIdAsync(int userId, int groupId);

    // Lấy giao dịch pending để xử lý (nếu cần job quét)
    Task<PaymentTransaction?> GetByProviderIdAsync(string providerId);
}