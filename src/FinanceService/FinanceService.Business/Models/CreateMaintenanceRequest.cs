namespace FinanceService.Business.Models;

public class CreateMaintenanceRequest
{
    public int VehicleId { get; set; }
    public int CoOwnerGroupId { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }

    // Danh sách chi tiết các hạng mục sửa chữa
    public List<MaintenanceDetailInput> Details { get; set; } = new();
}

public class MaintenanceDetailInput
{
    public string Service { get; set; } = string.Empty; // Tên dịch vụ (Thay nhớt, vá lốp...)
    public decimal Price { get; set; }
    public string? Description { get; set; }
}