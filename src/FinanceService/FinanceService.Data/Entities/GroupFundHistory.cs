namespace FinanceService.Data.Entities;

public class GroupFundHistory
{
    public int FundHistoryId { get; set; }

    public int FundId { get; set; }

    public decimal ChangeAmount { get; set; }

    public string? Reason { get; set; }

    public DateTime ChangedAt { get; set; }

    public int ChangedByUserId { get; set; }

    public virtual GroupFund Fund { get; set; } = null!;
}