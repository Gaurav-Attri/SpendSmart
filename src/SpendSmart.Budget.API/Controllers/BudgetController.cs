using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SpendSmart.Budget.API.Application.DTOs;
using SpendSmart.Budget.API.Application.Services;
using SpendSmart.Budget.API.Domain.Entities;

namespace SpendSmart.Budget.API.Controllers;

[ApiController]
[Route("api/budgets")]
[Authorize]
public class BudgetController : ControllerBase
{
    private readonly IBudgetPlanService _svc;

    public BudgetController(IBudgetPlanService svc) => _svc = svc;

    [HttpPost]
    [ProducesResponseType(typeof(BudgetPlan), 201)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Create([FromBody] CreateBudgetPlanDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var budget = await _svc.CreateBudgetPlanAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = budget.BudgetPlanId }, budget);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BudgetPlan), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(int id)
    {
        var budget = await _svc.GetBudgetPlanByIdAsync(id);
        return budget == null ? NotFound() : Ok(budget);
    }

    [HttpGet("user/{userId}")]
    [ProducesResponseType(typeof(List<BudgetPlan>), 200)]
    public async Task<IActionResult> GetByUser(int userId)
    {
        var budgets = await _svc.GetBudgetPlansByUserAsync(userId);
        return Ok(budgets);
    }

    [HttpGet("utilization/{budgetId}")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetUtilization(int budgetId)
    {
        try
        {
            var utilization = await _svc.GetBudgetUtilizationAsync(budgetId);
            return Ok(new { budgetId, utilizationPercentage = utilization });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpGet("alerts/{userId}")]
    [ProducesResponseType(typeof(List<BudgetPlan>), 200)]
    public async Task<IActionResult> GetAlerts(int userId)
    {
        var overBudget = await _svc.GetOverBudgetAlertsAsync(userId);
        return Ok(overBudget);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(204)]
    [ProducesResponseType(404)]
    [ProducesResponseType(400)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateBudgetPlanDto dto)
    {
        try
        {
            await _svc.UpdateBudgetPlanAsync(id, dto);
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
            await _svc.DeleteBudgetPlanAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}