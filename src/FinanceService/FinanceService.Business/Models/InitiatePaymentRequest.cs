namespace FinanceService.Business.Models;

public class InitiatePaymentRequest
{
    public int CoOwnerGroupId { get; set; }
    public int? SharedExpenseId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = "BANKING";
}