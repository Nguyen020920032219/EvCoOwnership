namespace FinanceService.Business.Models;

public class FundDto
{
    public int FundId { get; set; }
    public int CoOwnerGroupId { get; set; }
    public decimal Amount { get; set; }
}