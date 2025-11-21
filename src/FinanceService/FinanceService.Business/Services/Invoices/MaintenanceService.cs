using FinanceService.Business.Models;
using FinanceService.Data.Entities;
using FinanceService.Data.Repositories.Funds;
using FinanceService.Data.Repositories.Invoices;

namespace FinanceService.Business.Services.Invoices;

public class MaintenanceService : IMaintenanceService
{
    private readonly IFundRepository _fundRepo;
    private readonly IMaintenanceRepository _maintenanceRepo;

    public MaintenanceService(IMaintenanceRepository maintenanceRepo, IFundRepository fundRepo)
    {
        _maintenanceRepo = maintenanceRepo;
        _fundRepo = fundRepo;
    }

    // BƯỚC 1: TẠO BÁO GIÁ / HÓA ĐƠN TREO (Chưa trừ tiền)
    public async Task<MaintenanceInvoiceDto> CreateInvoiceAsync(int userId, CreateMaintenanceRequest request)
    {
        var totalAmount = request.Details.Sum(d => d.Price);
        if (totalAmount <= 0) throw new Exception("Tổng tiền hóa đơn phải lớn hơn 0.");

        // Chỉ tạo hóa đơn, KHÔNG trừ tiền quỹ ngay
        var invoice = new VehicleMaintenanceInvoice
        {
            CoOwnerGroupId = request.CoOwnerGroupId,
            VehicleId = request.VehicleId,
            Amount = totalAmount,
            Description = request.Description + " (Đang chờ duyệt chi)",
            InvoiceDate = request.InvoiceDate,
            CreatedByUserId = userId,
            FundId = null // QUAN TRỌNG: Null nghĩa là chưa thanh toán
        };

        foreach (var detail in request.Details)
            invoice.VehicleMaintenanceDetails.Add(new VehicleMaintenanceDetail
            {
                Service = detail.Service,
                Price = detail.Price,
                Description = detail.Description
            });

        await _maintenanceRepo.Add(invoice);
        // await _maintenanceRepo.DbContext().SaveChangesAsync();

        return MapToDto(invoice);
    }

    // BƯỚC 2: DUYỆT CHI (Sau khi Vote thông qua -> Gọi hàm này để trừ tiền)
    public async Task ApproveAndPayInvoiceAsync(int userId, int invoiceId)
    {
        // 1. Lấy hóa đơn
        var invoice = await _maintenanceRepo.GetByIdAsync(invoiceId);
        if (invoice == null) throw new Exception("Không tìm thấy hóa đơn.");

        // Check xem đã trả chưa
        if (invoice.FundId != null) throw new Exception("Hóa đơn này đã được thanh toán rồi.");

        // 2. Lấy quỹ
        var fund = await _fundRepo.GetByGroupIdAsync(invoice.CoOwnerGroupId);
        if (fund == null) throw new Exception("Nhóm chưa có quỹ.");

        // 3. Check số dư
        if (fund.Amount < invoice.Amount)
            throw new Exception($"Số dư quỹ không đủ (Cần: {invoice.Amount}, Có: {fund.Amount})");

        // 4. TRỪ TIỀN
        fund.Amount -= invoice.Amount;
        await _fundRepo.Update(fund);

        // 5. Cập nhật trạng thái hóa đơn (Gán vào quỹ đã dùng)
        invoice.FundId = fund.FundId;
        invoice.Description = invoice.Description?.Replace("(Đang chờ duyệt chi)", "(Đã thanh toán)");
        await _maintenanceRepo.Update(invoice);

        // 6. Ghi lịch sử
        await _fundRepo.AddHistoryAsync(new GroupFundHistory
        {
            FundId = fund.FundId,
            ChangeAmount = -invoice.Amount,
            Reason = $"Duyệt chi bảo dưỡng xe (Mã HĐ: {invoiceId})",
            ChangedByUserId = userId,
            ChangedAt = DateTime.UtcNow
        });

        await _fundRepo.DbContext().SaveChangesAsync();
    }

    public async Task<List<MaintenanceInvoiceDto>> GetInvoicesByGroupAsync(int groupId)
    {
        var list = await _maintenanceRepo.GetByGroupIdAsync(groupId);
        return list.Select(MapToDto).ToList();
    }

    private static MaintenanceInvoiceDto MapToDto(VehicleMaintenanceInvoice i)
    {
        return new MaintenanceInvoiceDto
        {
            InvoiceId = i.MaintenanceInvoiceId,
            VehicleId = i.VehicleId,
            TotalAmount = i.Amount,
            Description = i.Description ?? "",
            InvoiceDate = i.InvoiceDate,
            // Trả về trạng thái để FE biết
            // Status = i.FundId == null ? "Pending" : "Paid", 
            Details = i.VehicleMaintenanceDetails.Select(d => new MaintenanceDetailDto
            {
                Service = d.Service,
                Price = d.Price
            }).ToList()
        };
    }
}