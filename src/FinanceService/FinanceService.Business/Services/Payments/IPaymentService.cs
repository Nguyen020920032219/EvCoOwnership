using FinanceService.Business.Models;

namespace FinanceService.Business.Services.Payments;

public interface IPaymentService
{
    // Bước 1: Tạo giao dịch chờ
    Task<PaymentTransactionDto> InitiatePaymentAsync(int userId, InitiatePaymentRequest request);

    // Bước 2: Giả lập thanh toán thành công (Admin hoặc Dev tool dùng)
    Task ConfirmPaymentSuccessAsync(int transactionId);

    Task<List<PaymentTransactionDto>> GetMyHistoryAsync(int userId, int groupId);
}