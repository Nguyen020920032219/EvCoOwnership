namespace FinanceService.Data.Entities;

public class GroupFund
{
    public int FundId { get; set; }

    public int CoOwnerGroupId { get; set; }

    public decimal Amount { get; set; }

    public virtual ICollection<GroupFundHistory> GroupFundHistories { get; set; } = new List<GroupFundHistory>();

    public virtual ICollection<VehicleMaintenanceInvoice> VehicleMaintenanceInvoices { get; set; } =
        new List<VehicleMaintenanceInvoice>();
}