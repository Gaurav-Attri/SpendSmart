using SpendSmart.Expense.API.Domain.Entities;

namespace SpendSmart.Expense.API.Infrastructure.Repositories;

public interface IExpenseEntryRepository
{
    Task<ExpenseEntry?> GetByIdAsync(int id);
    Task<List<ExpenseEntry>> GetByUserIdAsync(int userId);
    Task<List<ExpenseEntry>> GetByDateRangeAsync(int userId, DateTime start, DateTime end);
    Task<List<ExpenseEntry>> GetByUserIdAndCategoryAsync(int userId, int categoryId);
    Task<List<ExpenseEntry>> SearchByDescriptionAsync(int userId, string keyword);
    Task<decimal> SumByUserIdAsync(int userId);
    Task<decimal> SumByUserIdAndDateRangeAsync(int userId, DateTime start, DateTime end);
    Task<List<ExpenseEntry>> GetRecurringAsync(int userId);
    Task AddAsync(ExpenseEntry expenseEntry);
    Task UpdateAsync(ExpenseEntry expenseEntry);
    Task DeleteAsync(ExpenseEntry expenseEntry);
    Task SaveChangesAsync();
}