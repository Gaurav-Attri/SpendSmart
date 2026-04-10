using SpendSmart.Income.API.Application.DTOs;
using SpendSmart.Income.API.Domain.Entities;

namespace SpendSmart.Income.API.Application.Services;

public interface IIncomeRecordService
{
    Task<IncomeRecord> AddIncomeRecordAsync(CreateIncomeRecordDto dto);
    Task<IncomeRecord?> GetIncomeRecordByIdAsync(int id);
    Task<List<IncomeRecord>> GetIncomeRecordsByUserAsync(int userId);
    Task<List<IncomeRecord>> GetByDateRangeAsync(int userId, DateTime start, DateTime end);
    Task<decimal> GetTotalIncomeAsync(int userId);
    Task<decimal> GetTotalByDateRangeAsync(int userId, DateTime start, DateTime end);
    Task<decimal> GetNetBalanceAsync(int userId);
    Task<decimal> GetTotalBySourceAsync(int userId, string source);
    Task<List<IncomeRecord>> GetRecurringIncomeRecordsAsync(int userId);
    Task UpdateIncomeRecordAsync(int id, UpdateIncomeRecordDto dto);
    Task DeleteIncomeRecordAsync(int id);
}