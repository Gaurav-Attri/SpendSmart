using SpendSmart.Budget.API.Domain.Entities;

namespace SpendSmart.Budget.API.Infrastructure.Repositories;

public interface IBudgetPlanRepository
{
    Task<BudgetPlan?> GetByIdAsync(int id);
    Task<List<BudgetPlan>> GetByUserIdAsync(int userId);
    Task<List<BudgetPlan>> GetActiveByUserIdAsync(int userId);
    Task<List<BudgetPlan>> GetOverBudgetAsync(int userId);
    Task<BudgetPlan?> GetByCategoryAndPeriodAsync(int userId, int? categoryId, string period);
    Task AddAsync(BudgetPlan budgetPlan);
    Task UpdateAsync(BudgetPlan budgetPlan);
    Task UpdateSpentAmountAsync(int budgetPlanId, decimal amount);
    Task SaveChangesAsync();
}