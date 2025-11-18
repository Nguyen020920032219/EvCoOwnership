namespace FinanceService.Business.Models;

public class DepositRequest
{
    public int CoOwnerGroupId { get; set; }
    public decimal Amount { get; set; }
    public string Reason { get; set; } = "Nạp quỹ";
}