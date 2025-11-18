namespace FinanceService.Business.Models;

public class FundHistoryDto
{
    public decimal ChangeAmount { get; set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public int ChangedByUserId { get; set; }
}