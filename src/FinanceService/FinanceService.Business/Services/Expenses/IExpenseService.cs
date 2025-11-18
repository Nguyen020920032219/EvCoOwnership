using FinanceService.Business.Models;

namespace FinanceService.Business.Services.Expenses;

public interface IExpenseService
{
    Task<int> CreateExpenseAsync(int userId, CreateExpenseRequest request);
    // Task<List<ExpenseDto>> GetGroupExpensesAsync(int groupId); (Bạn tự thêm DTO nếu cần)
}