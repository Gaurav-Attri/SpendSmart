using SpendSmart.Expense.API.Application.DTOs;
using SpendSmart.Expense.API.Domain.Entities;

namespace SpendSmart.Expense.API.Application.Services;

public interface IExpenseEntryService
{
    Task<ExpenseEntry> AddExpenseEntryAsync(CreateExpenseEntryDto dto);
    Task<ExpenseEntry?> GetExpenseEntryByIdAsync(int id);
    Task<List<ExpenseEntry>> GetExpenseEntriesByUserAsync(int userId);
    Task<List<ExpenseEntry>> GetByDateRangeAsync(int userId, DateTime start, DateTime end);
    Task<List<ExpenseEntry>> GetByCategoryAsync(int userId, int categoryId);
    Task<List<ExpenseEntry>> SearchExpenseEntriesAsync(int userId, string keyword);
    Task<decimal> GetTotalByUserAsync(int userId);
    Task<decimal> GetTotalByDateRangeAsync(int userId, DateTime start, DateTime end);
    Task<List<ExpenseEntry>> GetRecurringExpenseEntriesAsync(int userId);
    Task UpdateExpenseEntryAsync(int id, UpdateExpenseEntryDto dto);
    Task DeleteExpenseEntryAsync(int id);
}