namespace FinanceService.Data.Entities;

public class PaymentTransaction
{
    public int PaymentTransactionId { get; set; }

    public int UserId { get; set; }

    public int CoOwnerGroupId { get; set; }

    public int? SharedExpenseId { get; set; }

    public decimal Amount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public int Status { get; set; }

    public string? ProviderTransactionId { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual SharedExpense? SharedExpense { get; set; }
}