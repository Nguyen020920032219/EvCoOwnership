namespace FinanceService.Data.Entities;

public class VehicleMaintenanceInvoice
{
    public int MaintenanceInvoiceId { get; set; }

    public decimal Amount { get; set; }

    public int VehicleId { get; set; }

    public string? Description { get; set; }

    public DateOnly InvoiceDate { get; set; }

    public int CoOwnerGroupId { get; set; }

    public int CreatedByUserId { get; set; }

    public int? FundId { get; set; }

    public virtual GroupFund? Fund { get; set; }

    public virtual ICollection<VehicleMaintenanceDetail> VehicleMaintenanceDetails { get; set; } =
        new List<VehicleMaintenanceDetail>();
}