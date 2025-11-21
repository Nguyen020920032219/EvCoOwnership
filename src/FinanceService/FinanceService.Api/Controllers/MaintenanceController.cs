using System.Net.Http.Headers;
using System.Security.Claims;
using EvCoOwnership.Shared.Models;
using FinanceService.Business.Models;
using FinanceService.Business.Services.Invoices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MaintenanceController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IMaintenanceService _service;

    public MaintenanceController(IMaintenanceService service, IHttpClientFactory httpClientFactory)
    {
        _service = service;
        _httpClientFactory = httpClientFactory;
    }

    [HttpPost("create-proposal")]
    public async Task<IActionResult> CreateProposal([FromBody] CreateMaintenanceRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // 1. Tạo hóa đơn (Pending)
            var invoice = await _service.CreateInvoiceAsync(userId, request);

            // 2. GỌI SANG CONTRACT SERVICE ĐỂ TẠO VOTE TỰ ĐỘNG
            // Lấy token hiện tại để chuyển tiếp quyền
            var accessToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5196");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var voteRequest = new
            {
                request.CoOwnerGroupId,
                Topic = $"Duyệt chi bảo dưỡng xe ngày {request.InvoiceDate:dd/MM/yyyy}",
                Description =
                    $"[AUTO_INVOICE:{invoice.InvoiceId}] Tổng tiền: {invoice.TotalAmount:N0} VNĐ. Nội dung: {invoice.Description}"
            };

            var response = await client.PostAsJsonAsync("api/votes", voteRequest);

            if (response.IsSuccessStatusCode)
                return Ok(ApiResult<MaintenanceInvoiceDto>.Ok(invoice, "Đã tạo hóa đơn và tự động tạo cuộc bỏ phiếu."));

            // Nếu tạo vote lỗi thì vẫn trả về invoice nhưng kèm cảnh báo
            return Ok(ApiResult<MaintenanceInvoiceDto>.Ok(invoice,
                "Đã tạo hóa đơn, nhưng lỗi khi tạo Vote tự động. Vui lòng tạo vote thủ công."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }

    // 2. Admin/Operator gọi API này để "Duyệt chi" (Sau khi Vote thành công)
    [HttpPost("{invoiceId}/pay")]
    public async Task<IActionResult> PayInvoice(int invoiceId)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            // Có thể check thêm Role Admin ở đây nếu muốn chặt chẽ

            await _service.ApproveAndPayInvoiceAsync(userId, invoiceId);
            return Ok(ApiResult<string>.Ok("OK", "Đã duyệt chi và trừ tiền quỹ thành công."));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }

    [HttpGet("group/{groupId}")]
    public async Task<IActionResult> GetByGroup(int groupId)
    {
        var result = await _service.GetInvoicesByGroupAsync(groupId);
        return Ok(ApiResult<List<MaintenanceInvoiceDto>>.Ok(result));
    }
}