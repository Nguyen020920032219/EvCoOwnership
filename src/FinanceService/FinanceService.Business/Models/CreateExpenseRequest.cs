namespace FinanceService.Business.Models;

public class CreateExpenseRequest
{
    public int CoOwnerGroupId { get; set; }
    public string ExpenseType { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Description { get; set; } = string.Empty;

    // Danh sách user và % (hoặc số tiền) họ phải đóng
    // Nếu ShareMode = OWNERSHIP, FE có thể gửi list này dựa trên tính toán trước
    public List<ExpenseShareInput> Shares { get; set; } = new();
}