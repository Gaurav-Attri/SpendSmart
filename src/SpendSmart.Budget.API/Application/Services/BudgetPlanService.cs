using SpendSmart.Budget.API.Application.DTOs;
using SpendSmart.Budget.API.Domain.Entities;
using SpendSmart.Budget.API.Infrastructure.HttpClients;
using SpendSmart.Budget.API.Infrastructure.Repositories;

namespace SpendSmart.Budget.API.Application.Services;

public class BudgetPlanService : IBudgetPlanService
{
    private readonly IBudgetPlanRepository _repo;
    private readonly INotificationHttpClient _notifClient;

    public BudgetPlanService(IBudgetPlanRepository repo, INotificationHttpClient notifClient)
    {
        _repo = repo;
        _notifClient = notifClient;
    }

    public async Task<BudgetPlan> CreateBudgetPlanAsync(CreateBudgetPlanDto dto)
    {
        var budgetPlan = new BudgetPlan
        {
            UserId = dto.UserId,
            CategoryId = dto.CategoryId,
            Name = dto.Name,
            LimitAmount = dto.LimitAmount,
            Currency = dto.Currency,
            Period = dto.Period,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsActive = true,
            SpentAmount = 0
        };

        await _repo.AddAsync(budgetPlan);
        await _repo.SaveChangesAsync();
        return budgetPlan;
    }

    public Task<List<BudgetPlan>> GetBudgetPlansByUserAsync(int userId) =>
        _repo.GetByUserIdAsync(userId);

    public Task<BudgetPlan?> GetBudgetPlanByIdAsync(int id) =>
        _repo.GetByIdAsync(id);

    public async Task CheckBudgetOnExpenseAsync(int userId, int categoryId, decimal amount)
    {
        var budgets = await _repo.GetActiveByUserIdAsync(userId);
        var relevantBudgets = budgets.Where(b => b.CategoryId == categoryId || b.CategoryId == null).ToList();

        foreach (var budget in relevantBudgets)
        {
            await _repo.UpdateSpentAmountAsync(budget.BudgetPlanId, amount);
            
            var updated = await _repo.GetByIdAsync(budget.BudgetPlanId);
            if (updated == null) continue;

            var utilization = updated.GetUtilizationPercentage();

            if (utilization >= 100m)
            {
                await _notifClient.SendBudgetAlertAsync(userId, "BUDGET_EXCEEDED", utilization, budget.BudgetPlanId);
            }
            else if (utilization >= 80m)
            {
                await _notifClient.SendBudgetAlertAsync(userId, "BUDGET_WARNING", utilization, budget.BudgetPlanId);
            }
        }
    }

    public async Task<decimal> GetBudgetUtilizationAsync(int budgetPlanId)
    {
        var budget = await _repo.GetByIdAsync(budgetPlanId)
            ?? throw new KeyNotFoundException($"BudgetPlan {budgetPlanId} not found.");
        return budget.GetUtilizationPercentage();
    }

    public Task<List<BudgetPlan>> GetOverBudgetAlertsAsync(int userId) =>
        _repo.GetOverBudgetAsync(userId);

    public async Task UpdateBudgetPlanAsync(int id, CreateBudgetPlanDto dto)
    {
        var budget = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"BudgetPlan {id} not found.");

        budget.CategoryId = dto.CategoryId;
        budget.Name = dto.Name;
        budget.LimitAmount = dto.LimitAmount;
        budget.Currency = dto.Currency;
        budget.Period = dto.Period;
        budget.StartDate = dto.StartDate;
        budget.EndDate = dto.EndDate;

        await _repo.UpdateAsync(budget);
        await _repo.SaveChangesAsync();
    }

    public async Task DeleteBudgetPlanAsync(int id)
    {
        var budget = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"BudgetPlan {id} not found.");
        budget.IsActive = false;
        await _repo.UpdateAsync(budget);
        await _repo.SaveChangesAsync();
    }
}