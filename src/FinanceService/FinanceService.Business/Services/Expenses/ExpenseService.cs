using FinanceService.Business.Models;
using FinanceService.Data.Entities;
using FinanceService.Data.Repositories.Expenses;

namespace FinanceService.Business.Services.Expenses;

public class ExpenseService : IExpenseService
{
    private readonly IExpenseRepository _expenseRepo;

    public ExpenseService(IExpenseRepository expenseRepo)
    {
        _expenseRepo = expenseRepo;
    }

    public async Task<int> CreateExpenseAsync(int userId, CreateExpenseRequest request)
    {
        // 1. Validate tổng tiền chia
        var sumShares = request.Shares.Sum(s => s.Amount);
        if (sumShares != request.TotalAmount)
            throw new Exception($"Tổng tiền chia ({sumShares}) không khớp với tổng hóa đơn ({request.TotalAmount})");

        // 2. Tạo Expense
        var expense = new SharedExpense
        {
            CoOwnerGroupId = request.CoOwnerGroupId,
            ExpenseType = request.ExpenseType,
            Amount = request.TotalAmount,
            ExpenseDate = DateOnly.FromDateTime(DateTime.Now),
            CreatedBy = userId,
            ShareMode = "CUSTOM" // Tạm để custom vì FE gửi list lên
        };

        // 3. Tạo Shares
        foreach (var item in request.Shares)
            expense.SharedExpenseShares.Add(new SharedExpenseShare
            {
                UserId = item.UserId,
                ShareAmount = item.Amount
            });

        // 4. Save
        await _expenseRepo.Add(expense);

        return expense.ExpenseId;
    }
}