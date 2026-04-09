using Microsoft.EntityFrameworkCore;
using SpendSmart.Expense.API.Domain.Entities;
using SpendSmart.Expense.API.Infrastructure.Data;

namespace SpendSmart.Expense.API.Infrastructure.Repositories;

public class ExpenseEntryRepository : IExpenseEntryRepository
{
    private readonly ExpenseDbContext _ctx;

    public ExpenseEntryRepository(ExpenseDbContext ctx)
    {
        _ctx = ctx;
    }

    public Task<ExpenseEntry?> GetByIdAsync(int id) =>
        _ctx.ExpenseEntries.FindAsync(id).AsTask();

    public Task<List<ExpenseEntry>> GetByUserIdAsync(int userId) =>
        _ctx.ExpenseEntries
            .Where(e => e.UserId == userId)
            .OrderByDescending(e => e.Date)
            .ToListAsync();

    public Task<List<ExpenseEntry>> GetByDateRangeAsync(int userId, DateTime start, DateTime end) =>
        _ctx.ExpenseEntries
            .Where(e => e.UserId == userId && e.Date >= start && e.Date <= end)
            .OrderByDescending(e => e.Date)
            .ToListAsync();

    public Task<List<ExpenseEntry>> GetByUserIdAndCategoryAsync(int userId, int categoryId) =>
        _ctx.ExpenseEntries
            .Where(e => e.UserId == userId && e.CategoryId == categoryId)
            .OrderByDescending(e => e.Date)
            .ToListAsync();

    public Task<List<ExpenseEntry>> SearchByDescriptionAsync(int userId, string keyword) =>
        _ctx.ExpenseEntries
            .Where(e => e.UserId == userId && EF.Functions.Like(e.Description, $"%{keyword}%"))
            .OrderByDescending(e => e.Date)
            .ToListAsync();

    public Task<decimal> SumByUserIdAsync(int userId) =>
        _ctx.ExpenseEntries
            .Where(e => e.UserId == userId)
            .SumAsync(e => e.Amount);

    public Task<decimal> SumByUserIdAndDateRangeAsync(int userId, DateTime start, DateTime end) =>
        _ctx.ExpenseEntries
            .Where(e => e.UserId == userId && e.Date >= start && e.Date <= end)
            .SumAsync(e => e.Amount);

    public Task<List<ExpenseEntry>> GetRecurringAsync(int userId) =>
        _ctx.ExpenseEntries
            .Where(e => e.UserId == userId && e.IsRecurring)
            .OrderByDescending(e => e.Date)
            .ToListAsync();

    public async Task AddAsync(ExpenseEntry expenseEntry) =>
        await _ctx.ExpenseEntries.AddAsync(expenseEntry);

    public Task UpdateAsync(ExpenseEntry expenseEntry)
    {
        _ctx.ExpenseEntries.Update(expenseEntry);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(ExpenseEntry expenseEntry)
    {
        _ctx.ExpenseEntries.Remove(expenseEntry);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() =>
        _ctx.SaveChangesAsync();
}