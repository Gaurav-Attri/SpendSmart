using SpendSmart.Budget.API.Application.DTOs;
using SpendSmart.Budget.API.Domain.Entities;

namespace SpendSmart.Budget.API.Application.Services;

public interface IBudgetPlanService
{
    Task<BudgetPlan> CreateBudgetPlanAsync(CreateBudgetPlanDto dto);
    Task<List<BudgetPlan>> GetBudgetPlansByUserAsync(int userId);
    Task<BudgetPlan?> GetBudgetPlanByIdAsync(int id);
    Task CheckBudgetOnExpenseAsync(int userId, int categoryId, decimal amount);
    Task<decimal> GetBudgetUtilizationAsync(int budgetPlanId);
    Task<List<BudgetPlan>> GetOverBudgetAlertsAsync(int userId);
    Task UpdateBudgetPlanAsync(int id, CreateBudgetPlanDto dto);
    Task DeleteBudgetPlanAsync(int id);
}