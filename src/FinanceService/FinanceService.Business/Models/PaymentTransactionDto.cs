namespace FinanceService.Business.Models;

public class PaymentTransactionDto
{
    public int TransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Deposit" or "Expense Payment"
    public DateTime CreatedAt { get; set; }
}