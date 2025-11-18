namespace FinanceService.Data.Entities;

public class SharedExpense
{
    public int ExpenseId { get; set; }

    public int CoOwnerGroupId { get; set; }

    public string ExpenseType { get; set; } = null!;

    public decimal Amount { get; set; }

    public string ShareMode { get; set; } = null!;

    public DateOnly ExpenseDate { get; set; }

    public int CreatedBy { get; set; }

    public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; } = new List<PaymentTransaction>();

    public virtual ICollection<SharedExpenseShare> SharedExpenseShares { get; set; } = new List<SharedExpenseShare>();
}