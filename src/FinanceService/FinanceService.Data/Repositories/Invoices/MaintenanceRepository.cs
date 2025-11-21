using EvCoOwnership.Shared.BaseRepository;
using FinanceService.Data.Configurations;
using FinanceService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinanceService.Data.Repositories.Invoices;

public class MaintenanceRepository : BaseRepository<FinanceDbContext, VehicleMaintenanceInvoice, int>,
    IMaintenanceRepository
{
    public MaintenanceRepository(FinanceDbContext context) : base(context)
    {
    }

    public async Task<IReadOnlyList<VehicleMaintenanceInvoice>> GetByGroupIdAsync(int groupId)
    {
        return await DbSet()
            .Include(i => i.VehicleMaintenanceDetails) // Load luôn chi tiết
            .Where(i => i.CoOwnerGroupId == groupId)
            .OrderByDescending(i => i.InvoiceDate)
            .ToListAsync();
    }
}