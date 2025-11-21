using FinanceService.Business.Helpers;
using FinanceService.Business.Models;
using FinanceService.Data.Entities;
using FinanceService.Data.Repositories.Funds;
using FinanceService.Data.Repositories.Payments;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FinanceService.Business.Services.Payments;

public class PaymentService : IPaymentService
{
    private readonly IFundRepository _fundRepo;
    private readonly IPaymentRepository _paymentRepo;

    public PaymentService(IPaymentRepository paymentRepo, IFundRepository fundRepo)
    {
        _paymentRepo = paymentRepo;
        _fundRepo = fundRepo;
    }

    // =================================================================================================
    // CÁC HÀM CORE (Dùng cho cả Fake Payment & Real Payment)
    // =================================================================================================

    // 1. Tạo giao dịch ban đầu (Pending)
    // API Fake-pay sẽ gọi hàm này đầu tiên để tạo bản ghi
    public async Task<PaymentTransactionDto> InitiatePaymentAsync(int userId, InitiatePaymentRequest request)
    {
        var trans = new PaymentTransaction
        {
            UserId = userId,
            CoOwnerGroupId = request.CoOwnerGroupId,
            SharedExpenseId = request.SharedExpenseId,
            Amount = request.Amount,
            PaymentMethod = request.PaymentMethod ?? "CASH_MOCK", // Mặc định nếu null
            Status = 0, // 0 = Pending
            CreatedAt = DateTime.UtcNow,
            ProviderTransactionId = Guid.NewGuid().ToString()
        };

        await _paymentRepo.Add(trans);

        // QUAN TRỌNG: Phải SaveChanges ngay để có ID trả về cho Controller
        await _paymentRepo.DbContext().SaveChangesAsync();

        return MapToDto(trans);
    }

    // 2. Xác nhận thành công & Cộng tiền (Logic cốt lõi)
    // API Fake-pay sẽ gọi hàm này ngay sau bước 1 để chốt đơn
    public async Task ConfirmPaymentSuccessAsync(int transactionId)
    {
        var trans = await _paymentRepo.GetByIdAsync(transactionId);
        if (trans == null) throw new Exception("Transaction not found");

        // Idempotency: Nếu đã thành công rồi thì thôi, tránh cộng tiền 2 lần
        if (trans.Status == 1) return;

        // --- A. Update trạng thái giao dịch ---
        trans.Status = 1; // 1 = Success
        trans.UpdatedAt = DateTime.UtcNow;
        await _paymentRepo.Update(trans);

        // --- B. Cộng tiền vào Quỹ chung ---
        var fund = await _fundRepo.GetByGroupIdAsync(trans.CoOwnerGroupId);
        if (fund == null)
        {
            // Nếu nhóm chưa có quỹ thì tạo mới
            fund = new GroupFund { CoOwnerGroupId = trans.CoOwnerGroupId, Amount = 0 };
            await _fundRepo.Add(fund);
        }

        fund.Amount += trans.Amount; // Cộng tiền
        await _fundRepo.Update(fund);

        // --- C. Ghi log lịch sử biến động số dư ---
        var reason = trans.SharedExpenseId.HasValue
            ? $"Thanh toán khoản chi #{trans.SharedExpenseId}"
            : "Nạp quỹ (Thanh toán giả lập)";

        await _fundRepo.AddHistoryAsync(new GroupFundHistory
        {
            FundId = fund.FundId,
            ChangeAmount = trans.Amount,
            Reason = reason,
            ChangedByUserId = trans.UserId,
            ChangedAt = DateTime.UtcNow
        });

        // Lưu tất cả thay đổi (Transaction, Fund, History) cùng lúc
        await _fundRepo.DbContext().SaveChangesAsync();
    }

    public async Task<List<PaymentTransactionDto>> GetMyHistoryAsync(int userId, int groupId)
    {
        var list = await _paymentRepo.GetByUserIdAsync(userId, groupId);
        return list.Select(MapToDto).ToList();
    }

    // =================================================================================================
    // CÁC HÀM VNPAY (Giữ lại để sau này dùng nếu cần, không ảnh hưởng luồng Fake)
    // =================================================================================================

    public async Task<string> CreateVnPayUrl(int userId, InitiatePaymentRequest request, HttpContext context,
        IConfiguration config)
    {
        // Reuse logic tạo transaction
        var transDto = await InitiatePaymentAsync(userId, request);
        var transactionId = transDto.TransactionId;

        // Cập nhật lại PaymentMethod cho đúng
        var trans = await _paymentRepo.GetByIdAsync(transactionId);
        if (trans != null)
        {
            trans.PaymentMethod = "VNPAY";
            await _paymentRepo.Update(trans);
            await _paymentRepo.DbContext().SaveChangesAsync();
        }

        // Build URL
        var vnpay = new VnPayLibrary();
        vnpay.AddRequestData("vnp_Version", "2.1.0");
        vnpay.AddRequestData("vnp_Command", "pay");
        vnpay.AddRequestData("vnp_TmnCode", config["VnPay:TmnCode"]);
        vnpay.AddRequestData("vnp_Amount", ((long)(request.Amount * 100)).ToString());
        vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", "VND");
        vnpay.AddRequestData("vnp_IpAddr", VnPayLibrary.GetIpAddress(context));
        vnpay.AddRequestData("vnp_Locale", "vn");
        vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang #{transactionId}");
        vnpay.AddRequestData("vnp_OrderType", "other");
        vnpay.AddRequestData("vnp_ReturnUrl", config["VnPay:ReturnUrl"]);
        vnpay.AddRequestData("vnp_TxnRef", transactionId.ToString());

        return vnpay.CreateRequestUrl(config["VnPay:BaseUrl"], config["VnPay:HashSecret"]);
    }

    public async Task<(bool Success, string Message, int TransactionId)> ProcessVnPayReturnAsync(
        IQueryCollection collections, IConfiguration config)
    {
        var vnpay = new VnPayLibrary();
        foreach (var (key, value) in collections)
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                vnpay.AddResponseData(key, value.ToString());

        var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
        var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
        var vnp_TxnRef = vnpay.GetResponseData("vnp_TxnRef");

        if (!int.TryParse(vnp_TxnRef, out var transactionId)) return (false, "Invalid Transaction Id", 0);

        var checkSignature = vnpay.ValidateSignature(vnp_SecureHash, config["VnPay:HashSecret"]);
        if (!checkSignature) return (false, "Invalid Signature", transactionId);

        if (vnp_ResponseCode == "00")
        {
            await ConfirmPaymentSuccessAsync(transactionId);
            return (true, "Success", transactionId);
        }

        var trans = await _paymentRepo.GetByIdAsync(transactionId);
        if (trans != null)
        {
            trans.Status = 2; // Failed
            await _paymentRepo.Update(trans);
            await _paymentRepo.DbContext().SaveChangesAsync();
        }

        return (false, $"Error: {vnp_ResponseCode}", transactionId);
    }

    private static PaymentTransactionDto MapToDto(PaymentTransaction t)
    {
        return new PaymentTransactionDto
        {
            TransactionId = t.PaymentTransactionId,
            Amount = t.Amount,
            Status = t.Status == 0 ? "Pending" : t.Status == 1 ? "Success" : "Failed",
            Type = t.SharedExpenseId.HasValue ? "Expense" : "Deposit",
            CreatedAt = t.CreatedAt
        };
    }
}