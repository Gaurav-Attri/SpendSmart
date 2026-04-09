using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Expense.API.Application.DTOs;
using SpendSmart.Expense.API.Application.Services;
using SpendSmart.Expense.API.Domain.Entities;

namespace SpendSmart.Expense.API.Controllers;

[ApiController]
[Route("api/expenses")]
[Authorize]
public class ExpenseController : ControllerBase
{
    private readonly IExpenseEntryService _svc;

    public ExpenseController(IExpenseEntryService svc) => _svc = svc;

    [HttpPost]
    [ProducesResponseType(typeof(ExpenseEntry), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateExpenseEntryDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var expenseEntry = await _svc.AddExpenseEntryAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = expenseEntry.ExpenseEntryId }, expenseEntry);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ExpenseEntry), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var expenseEntry = await _svc.GetExpenseEntryByIdAsync(id);
        return expenseEntry == null ? NotFound() : Ok(expenseEntry);
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(List<ExpenseEntry>), 200)]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var expenseEntries = await _svc.GetExpenseEntriesByUserAsync(userId);
        return Ok(expenseEntries);
    }

    [HttpGet("filter")]
    [ProducesResponseType(typeof(List<ExpenseEntry>), 200)]
    public async Task<IActionResult> Filter(
        [FromQuery] int userId,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        if (start > end)
            return BadRequest(new { error = "Start date must be before end date" });

        var expenseEntries = await _svc.GetByDateRangeAsync(userId, start, end);
        return Ok(expenseEntries);
    }

    [HttpGet("category/{userId}/{categoryId}")]
    [ProducesResponseType(typeof(List<ExpenseEntry>), 200)]
    public async Task<IActionResult> GetByCategory(int userId, int categoryId)
    {
        var expenseEntries = await _svc.GetByCategoryAsync(userId, categoryId);
        return Ok(expenseEntries);
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(List<ExpenseEntry>), 200)]
    public async Task<IActionResult> Search([FromQuery] int userId, [FromQuery] string q)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest(new { error = "Search keyword is required" });

        var expenseEntries = await _svc.SearchExpenseEntriesAsync(userId, q);
        return Ok(expenseEntries);
    }

    [HttpGet("recurring/{userId}")]
    [ProducesResponseType(typeof(List<ExpenseEntry>), 200)]
    public async Task<IActionResult> GetRecurring(int userId)
    {
        var expenseEntries = await _svc.GetRecurringExpenseEntriesAsync(userId);
        return Ok(expenseEntries);
    }

    [HttpGet("total/{userId}")]
    [ProducesResponseType(typeof(decimal), 200)]
    public async Task<IActionResult> GetTotal(int userId)
    {
        var total = await _svc.GetTotalByUserAsync(userId);
        return Ok(total);
    }

    [HttpGet("total-by-date")]
    [ProducesResponseType(typeof(decimal), 200)]
    public async Task<IActionResult> GetTotalByDateRange(
        [FromQuery] int userId,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        if (start > end)
            return BadRequest(new { error = "Start date must be before end date" });

        var total = await _svc.GetTotalByDateRangeAsync(userId, start, end);
        return Ok(total);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateExpenseEntryDto dto)
    {
        try
        {
            await _svc.UpdateExpenseEntryAsync(id, dto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _svc.DeleteExpenseEntryAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}