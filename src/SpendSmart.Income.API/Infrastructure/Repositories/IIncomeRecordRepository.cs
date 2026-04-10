using SpendSmart.Income.API.Domain.Entities;

namespace SpendSmart.Income.API.Infrastructure.Repositories;

public interface IIncomeRecordRepository
{
    Task<IncomeRecord?> GetByIdAsync(int id);
    Task<List<IncomeRecord>> GetByUserIdAsync(int userId);
    Task<List<IncomeRecord>> GetByDateRangeAsync(int userId, DateTime start, DateTime end);
    Task<decimal> SumByUserIdAsync(int userId);
    Task<decimal> SumByUserIdAndDateRangeAsync(int userId, DateTime start, DateTime end);
    Task<decimal> SumBySourceAsync(int userId, string source);
    Task<List<IncomeRecord>> GetRecurringAsync(int userId);
    Task AddAsync(IncomeRecord incomeRecord);
    Task UpdateAsync(IncomeRecord incomeRecord);
    Task DeleteAsync(IncomeRecord incomeRecord);
    Task SaveChangesAsync();
}