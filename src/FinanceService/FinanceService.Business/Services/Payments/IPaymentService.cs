using FinanceService.Business.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FinanceService.Business.Services.Payments;

public interface IPaymentService
{
    // Các hàm cho Fake Payment
    Task<PaymentTransactionDto> InitiatePaymentAsync(int userId, InitiatePaymentRequest request);
    Task ConfirmPaymentSuccessAsync(int transactionId);
    Task<List<PaymentTransactionDto>> GetMyHistoryAsync(int userId, int groupId);

    // Các hàm cho VNPAY (Giữ lại)
    Task<string> CreateVnPayUrl(int userId, InitiatePaymentRequest request, HttpContext context, IConfiguration config);

    Task<(bool Success, string Message, int TransactionId)> ProcessVnPayReturnAsync(IQueryCollection collections,
        IConfiguration config);
}