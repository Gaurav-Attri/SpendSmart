using SpendSmart.Report.API.Application.DTOs;

namespace SpendSmart.Report.API.Infrastructure.HttpClients;

public interface IExpenseHttpClient
{
    Task<List<ExpenseDto>> GetByUserAsync(int userId, string token);
    Task<List<ExpenseDto>> GetByDateRangeAsync(int userId, DateTime start, DateTime end, string token);
    Task<decimal> GetTotalByUserAsync(int userId, string token);
}