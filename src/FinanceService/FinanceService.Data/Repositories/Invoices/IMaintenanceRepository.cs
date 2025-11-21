using EvCoOwnership.Shared.BaseRepository;
using FinanceService.Data.Entities;

namespace FinanceService.Data.Repositories.Invoices;

public interface IMaintenanceRepository : IBaseRepository<VehicleMaintenanceInvoice, int>
{
    // Lấy danh sách hóa đơn của 1 nhóm
    Task<IReadOnlyList<VehicleMaintenanceInvoice>> GetByGroupIdAsync(int groupId);
}