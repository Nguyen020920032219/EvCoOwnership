namespace FinanceService.Business.Models;

public class MaintenanceInvoiceDto
{
    public int InvoiceId { get; set; }
    public int VehicleId { get; set; }
    public decimal TotalAmount { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateOnly InvoiceDate { get; set; }
    public List<MaintenanceDetailDto> Details { get; set; } = new();
}

public class MaintenanceDetailDto
{
    public string Service { get; set; } = string.Empty;
    public decimal Price { get; set; }
}