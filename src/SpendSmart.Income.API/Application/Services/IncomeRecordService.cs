using SpendSmart.Income.API.Application.DTOs;
using SpendSmart.Income.API.Domain.Entities;
using SpendSmart.Income.API.Infrastructure.HttpClients;
using SpendSmart.Income.API.Infrastructure.Repositories;

namespace SpendSmart.Income.API.Application.Services;

public class IncomeRecordService : IIncomeRecordService
{
    private readonly IIncomeRecordRepository _repo;
    private readonly IExpenseHttpClient _expenseClient;

    public IncomeRecordService(IIncomeRecordRepository repo, IExpenseHttpClient expenseClient)
    {
        _repo = repo;
        _expenseClient = expenseClient;
    }

    public async Task<IncomeRecord> AddIncomeRecordAsync(CreateIncomeRecordDto dto)
    {
        var incomeRecord = new IncomeRecord
        {
            UserId = dto.UserId,
            Source = dto.Source,
            Amount = dto.Amount,
            Currency = dto.Currency,
            Description = dto.Description,
            Date = dto.Date,
            IsRecurring = dto.IsRecurring,
            RecurrenceType = dto.RecurrenceType,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(incomeRecord);
        await _repo.SaveChangesAsync();
        return incomeRecord;
    }

    public Task<IncomeRecord?> GetIncomeRecordByIdAsync(int id) =>
        _repo.GetByIdAsync(id);

    public Task<List<IncomeRecord>> GetIncomeRecordsByUserAsync(int userId) =>
        _repo.GetByUserIdAsync(userId);

    public Task<List<IncomeRecord>> GetByDateRangeAsync(int userId, DateTime start, DateTime end) =>
        _repo.GetByDateRangeAsync(userId, start, end);

    public Task<decimal> GetTotalIncomeAsync(int userId) =>
        _repo.SumByUserIdAsync(userId);

    public Task<decimal> GetTotalByDateRangeAsync(int userId, DateTime start, DateTime end) =>
        _repo.SumByUserIdAndDateRangeAsync(userId, start, end);

    public async Task<decimal> GetNetBalanceAsync(int userId)
    {
        var totalIncome = await GetTotalIncomeAsync(userId);
        var totalExpense = await _expenseClient.GetTotalByUserAsync(userId);
        return totalIncome - totalExpense;
    }

    public Task<decimal> GetTotalBySourceAsync(int userId, string source) =>
        _repo.SumBySourceAsync(userId, source);

    public Task<List<IncomeRecord>> GetRecurringIncomeRecordsAsync(int userId) =>
        _repo.GetRecurringAsync(userId);

    public async Task UpdateIncomeRecordAsync(int id, UpdateIncomeRecordDto dto)
    {
        var incomeRecord = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"IncomeRecord {id} not found.");

        if (dto.Source != null) incomeRecord.Source = dto.Source;
        if (dto.Amount.HasValue) incomeRecord.Amount = dto.Amount.Value;
        if (dto.Currency != null) incomeRecord.Currency = dto.Currency;
        if (dto.Description != null) incomeRecord.Description = dto.Description;
        if (dto.Date.HasValue) incomeRecord.Date = dto.Date.Value;
        if (dto.IsRecurring.HasValue) incomeRecord.IsRecurring = dto.IsRecurring.Value;
        if (dto.RecurrenceType != null) incomeRecord.RecurrenceType = dto.RecurrenceType;

        incomeRecord.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(incomeRecord);
        await _repo.SaveChangesAsync();
    }

    public async Task DeleteIncomeRecordAsync(int id)
    {
        var incomeRecord = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"IncomeRecord {id} not found.");
        await _repo.DeleteAsync(incomeRecord);
        await _repo.SaveChangesAsync();
    }
}