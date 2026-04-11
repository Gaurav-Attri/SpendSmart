using Microsoft.EntityFrameworkCore;
using SpendSmart.Budget.API.Domain.Entities;
using SpendSmart.Budget.API.Infrastructure.Data;

namespace SpendSmart.Budget.API.Infrastructure.Repositories;

public class BudgetPlanRepository : IBudgetPlanRepository
{
    private readonly BudgetDbContext _ctx;

    public BudgetPlanRepository(BudgetDbContext ctx)
    {
        _ctx = ctx;
    }

    public Task<BudgetPlan?> GetByIdAsync(int id) =>
        _ctx.BudgetPlans.FindAsync(id).AsTask();

    public Task<List<BudgetPlan>> GetByUserIdAsync(int userId) =>
        _ctx.BudgetPlans
            .Where(b => b.UserId == userId)
            .OrderBy(b => b.Name)
            .ToListAsync();

    public Task<List<BudgetPlan>> GetActiveByUserIdAsync(int userId) =>
        _ctx.BudgetPlans
            .Where(b => b.UserId == userId && b.IsActive)
            .ToListAsync();

    public Task<List<BudgetPlan>> GetOverBudgetAsync(int userId) =>
        _ctx.BudgetPlans
            .Where(b => b.UserId == userId && b.SpentAmount > b.LimitAmount)
            .ToListAsync();

    public Task<BudgetPlan?> GetByCategoryAndPeriodAsync(int userId, int? categoryId, string period) =>
        _ctx.BudgetPlans
            .FirstOrDefaultAsync(b => b.UserId == userId && b.CategoryId == categoryId && b.Period == period && b.IsActive);

    public async Task AddAsync(BudgetPlan budgetPlan) =>
        await _ctx.BudgetPlans.AddAsync(budgetPlan);

    public Task UpdateAsync(BudgetPlan budgetPlan)
    {
        _ctx.BudgetPlans.Update(budgetPlan);
        return Task.CompletedTask;
    }

    public Task UpdateSpentAmountAsync(int budgetPlanId, decimal amount) =>
        _ctx.BudgetPlans
            .Where(b => b.BudgetPlanId == budgetPlanId)
            .ExecuteUpdateAsync(s => s.SetProperty(b => b.SpentAmount, b => b.SpentAmount + amount));

    public Task SaveChangesAsync() =>
        _ctx.SaveChangesAsync();
}