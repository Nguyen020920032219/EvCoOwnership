using System.Security.Claims;
using EvCoOwnership.Shared.Models;
using FinanceService.Business.Models;
using FinanceService.Business.Services.Expenses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceService.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ExpensesController : ControllerBase
{
    private readonly IExpenseService _expenseService;

    public ExpensesController(IExpenseService expenseService)
    {
        _expenseService = expenseService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var expenseId = await _expenseService.CreateExpenseAsync(userId, request);
            return Ok(ApiResult<int>.Ok(expenseId, "Tạo khoản chi thành công"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResult<string>.Fail(ex.Message));
        }
    }
}