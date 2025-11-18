namespace FinanceService.Data.Entities;

public class SharedExpenseShare
{
    public int ExpenseShareId { get; set; }

    public int ExpenseId { get; set; }

    public int UserId { get; set; }

    public decimal ShareAmount { get; set; }

    public virtual SharedExpense Expense { get; set; } = null!;
}