namespace FinanceService.Data.Entities;

public class VehicleMaintenanceDetail
{
    public int MaintenanceDetailId { get; set; }

    public string Service { get; set; } = null!;

    public decimal Price { get; set; }

    public string? Description { get; set; }

    public int MaintenanceInvoiceId { get; set; }

    public virtual VehicleMaintenanceInvoice MaintenanceInvoice { get; set; } = null!;
}