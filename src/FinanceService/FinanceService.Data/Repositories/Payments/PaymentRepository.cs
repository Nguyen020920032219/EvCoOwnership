using EvCoOwnership.Shared.BaseRepository;
using FinanceService.Data.Configurations;
using FinanceService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Data.Repositories.Payments;

public class PaymentRepository : BaseRepository<FinanceDbContext, PaymentTransaction, int>, IPaymentRepository
{
    public PaymentRepository(FinanceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<PaymentTransaction>> GetByUserIdAsync(int userId, int groupId)
    {
        return await DbSet()
            .Where(p => p.UserId == userId && p.CoOwnerGroupId == groupId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    public async Task<PaymentTransaction?> GetByProviderIdAsync(string providerId)
    {
        return await DbSet().FirstOrDefaultAsync(p => p.ProviderTransactionId == providerId);
    }
}