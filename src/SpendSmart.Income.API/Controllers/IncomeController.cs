using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Income.API.Application.DTOs;
using SpendSmart.Income.API.Application.Services;
using SpendSmart.Income.API.Domain.Entities;

namespace SpendSmart.Income.API.Controllers;

[ApiController]
[Route("api/incomes")]
[Authorize]
public class IncomeController : ControllerBase
{
    private readonly IIncomeRecordService _svc;

    public IncomeController(IIncomeRecordService svc) => _svc = svc;

    [HttpPost]
    [ProducesResponseType(typeof(IncomeRecord), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateIncomeRecordDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var incomeRecord = await _svc.AddIncomeRecordAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = incomeRecord.IncomeRecordId }, incomeRecord);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(IncomeRecord), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var incomeRecord = await _svc.GetIncomeRecordByIdAsync(id);
        return incomeRecord == null ? NotFound() : Ok(incomeRecord);
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(List<IncomeRecord>), 200)]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var incomeRecords = await _svc.GetIncomeRecordsByUserAsync(userId);
        return Ok(incomeRecords);
    }

    [HttpGet("filter")]
    [ProducesResponseType(typeof(List<IncomeRecord>), 200)]
    public async Task<IActionResult> Filter(
        [FromQuery] int userId,
        [FromQuery] DateTime start,
        [FromQuery] DateTime end)
    {
        if (start > end)
            return BadRequest(new { error = "Start date must be before end date" });

        var incomeRecords = await _svc.GetByDateRangeAsync(userId, start, end);
        return Ok(incomeRecords);
    }

    [HttpGet("net-balance/{userId}")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> GetNetBalance(int userId)
    {
        var netBalance = await _svc.GetNetBalanceAsync(userId);
        return Ok(new { userId, netBalance });
    }

    [HttpGet("by-source/{userId}/{source}")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<IActionResult> GetBySource(int userId, string source)
    {
        var total = await _svc.GetTotalBySourceAsync(userId, source);
        return Ok(new { userId, source, total });
    }

    [HttpGet("recurring/{userId}")]
    [ProducesResponseType(typeof(List<IncomeRecord>), 200)]
    public async Task<IActionResult> GetRecurring(int userId)
    {
        var incomeRecords = await _svc.GetRecurringIncomeRecordsAsync(userId);
        return Ok(incomeRecords);
    }

    [HttpGet("total/{userId}")]
    [ProducesResponseType(typeof(decimal), 200)]
    public async Task<IActionResult> GetTotal(int userId)
    {
        var total = await _svc.GetTotalIncomeAsync(userId);
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
    public async Task<IActionResult> Update(int id, [FromBody] UpdateIncomeRecordDto dto)
    {
        try
        {
            await _svc.UpdateIncomeRecordAsync(id, dto);
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
            await _svc.DeleteIncomeRecordAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}