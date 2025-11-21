using System.Security.Claims;
using EvCoOwnership.Shared.Models;
using FinanceService.Business.Models;
using FinanceService.Business.Services.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    // Bỏ bớt config nếu không dùng VNPAY nữa
    // private readonly IConfiguration _configuration; 

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    // --- API THANH TOÁN GIẢ LẬP (MỚI) ---
    // Gọi cái là thành công luôn, tiền vào quỹ ngay lập tức
    [HttpPost("fake-pay")]
    public async Task<IActionResult> FakePayment([FromBody] InitiatePaymentRequest request)
    {
        try
        {
            var userIdStr = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();
            var userId = int.Parse(userIdStr);

            // Bước 1: Tạo giao dịch (Pending)
            var transDto = await _paymentService.InitiatePaymentAsync(userId, request);

            // Bước 2: Tự động xác nhận thành công (Success + Cộng tiền quỹ)
            await _paymentService.ConfirmPaymentSuccessAsync(transDto.TransactionId);

            return Ok(ApiResult<string>.Ok("OK", "Thanh toán giả lập thành công! Tiền đã vào quỹ."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }

    // API xem lịch sử (Giữ lại để check kết quả)
    [HttpGet("my-history")]
    public async Task<IActionResult> GetHistory([FromQuery] int groupId)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var list = await _paymentService.GetMyHistoryAsync(userId, groupId);
            return Ok(ApiResult<List<PaymentTransactionDto>>.Ok(list));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<List<PaymentTransactionDto>>.Fail(ex.Message));
        }
    }
}