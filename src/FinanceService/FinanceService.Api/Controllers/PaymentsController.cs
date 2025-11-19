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

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("initiate")]
    public async Task<IActionResult> Initiate([FromBody] InitiatePaymentRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await _paymentService.InitiatePaymentAsync(userId, request);
            return Ok(ApiResult<PaymentTransactionDto>.Ok(result, "Khởi tạo giao dịch thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }

    // API này thường sẽ là Webhook từ VNPay, ở đây làm dạng manual confirm để test
    [HttpPost("{transactionId}/confirm-mock")]
    public async Task<IActionResult> ConfirmMock(int transactionId)
    {
        try
        {
            await _paymentService.ConfirmPaymentSuccessAsync(transactionId);
            return Ok(ApiResult<string>.Ok("OK", "Giao dịch đã được xác nhận thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }

    [HttpGet("my-history")]
    public async Task<IActionResult> GetHistory([FromQuery] int groupId)
    {
        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
        var list = await _paymentService.GetMyHistoryAsync(userId, groupId);
        return Ok(ApiResult<List<PaymentTransactionDto>>.Ok(list));
    }
}