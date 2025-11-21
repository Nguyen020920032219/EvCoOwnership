using FinanceService.Business.Models;

namespace FinanceService.Business.Services.Invoices;

public interface IMaintenanceService
{
    Task<MaintenanceInvoiceDto> CreateInvoiceAsync(int userId, CreateMaintenanceRequest request);

    // Hàm mới để duyệt chi
    Task ApproveAndPayInvoiceAsync(int userId, int invoiceId);

    Task<List<MaintenanceInvoiceDto>> GetInvoicesByGroupAsync(int groupId);
}