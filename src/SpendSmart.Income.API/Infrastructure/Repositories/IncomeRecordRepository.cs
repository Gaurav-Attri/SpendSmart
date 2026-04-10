using Microsoft.EntityFrameworkCore;
using SpendSmart.Income.API.Domain.Entities;
using SpendSmart.Income.API.Infrastructure.Data;

namespace SpendSmart.Income.API.Infrastructure.Repositories;

public class IncomeRecordRepository : IIncomeRecordRepository
{
    private readonly IncomeDbContext _ctx;

    public IncomeRecordRepository(IncomeDbContext ctx)
    {
        _ctx = ctx;
    }

    public Task<IncomeRecord?> GetByIdAsync(int id) =>
        _ctx.IncomeRecords.FindAsync(id).AsTask();

    public Task<List<IncomeRecord>> GetByUserIdAsync(int userId) =>
        _ctx.IncomeRecords
            .Where(i => i.UserId == userId)
            .OrderByDescending(i => i.Date)
            .ToListAsync();

    public Task<List<IncomeRecord>> GetByDateRangeAsync(int userId, DateTime start, DateTime end) =>
        _ctx.IncomeRecords
            .Where(i => i.UserId == userId && i.Date >= start && i.Date <= end)
            .OrderByDescending(i => i.Date)
            .ToListAsync();

    public Task<decimal> SumByUserIdAsync(int userId) =>
        _ctx.IncomeRecords
            .Where(i => i.UserId == userId)
            .SumAsync(i => i.Amount);

    public Task<decimal> SumByUserIdAndDateRangeAsync(int userId, DateTime start, DateTime end) =>
        _ctx.IncomeRecords
            .Where(i => i.UserId == userId && i.Date >= start && i.Date <= end)
            .SumAsync(i => i.Amount);

    public Task<decimal> SumBySourceAsync(int userId, string source) =>
        _ctx.IncomeRecords
            .Where(i => i.UserId == userId && i.Source == source)
            .SumAsync(i => i.Amount);

    public Task<List<IncomeRecord>> GetRecurringAsync(int userId) =>
        _ctx.IncomeRecords
            .Where(i => i.UserId == userId && i.IsRecurring)
            .OrderByDescending(i => i.Date)
            .ToListAsync();

    public async Task AddAsync(IncomeRecord incomeRecord) =>
        await _ctx.IncomeRecords.AddAsync(incomeRecord);

    public Task UpdateAsync(IncomeRecord incomeRecord)
    {
        _ctx.IncomeRecords.Update(incomeRecord);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(IncomeRecord incomeRecord)
    {
        _ctx.IncomeRecords.Remove(incomeRecord);
        return Task.CompletedTask;
    }

    public Task SaveChangesAsync() =>
        _ctx.SaveChangesAsync();
}