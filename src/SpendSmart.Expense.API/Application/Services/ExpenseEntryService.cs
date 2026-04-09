using MassTransit;
using SpendSmart.Contracts.Events;
using SpendSmart.Expense.API.Application.DTOs;
using SpendSmart.Expense.API.Domain.Entities;
using SpendSmart.Expense.API.Infrastructure.Repositories;

namespace SpendSmart.Expense.API.Application.Services;

public class ExpenseEntryService : IExpenseEntryService
{
    private readonly IExpenseEntryRepository _repo;
    private readonly IPublishEndpoint _bus;

    public ExpenseEntryService(IExpenseEntryRepository repo, IPublishEndpoint bus)
    {
        _repo = repo;
        _bus = bus;
    }

    public async Task<ExpenseEntry> AddExpenseEntryAsync(CreateExpenseEntryDto dto)
    {
        var expenseEntry = new ExpenseEntry
        {
            UserId = dto.UserId,
            CategoryId = dto.CategoryId,
            Amount = dto.Amount,
            Currency = dto.Currency,
            Description = dto.Description,
            Date = dto.Date,
            PaymentMode = dto.PaymentMode,
            Tags = dto.Tags,
            IsRecurring = dto.IsRecurring,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(expenseEntry);
        await _repo.SaveChangesAsync();

        // Publish event for Budget.API consumer
        await _bus.Publish(new ExpenseCreatedEvent(
            dto.UserId,
            dto.CategoryId,
            dto.Amount,
            dto.Currency
        ));

        return expenseEntry;
    }

    public Task<ExpenseEntry?> GetExpenseEntryByIdAsync(int id) =>
        _repo.GetByIdAsync(id);

    public Task<List<ExpenseEntry>> GetExpenseEntriesByUserAsync(int userId) =>
        _repo.GetByUserIdAsync(userId);

    public Task<List<ExpenseEntry>> GetByDateRangeAsync(int userId, DateTime start, DateTime end) =>
        _repo.GetByDateRangeAsync(userId, start, end);

    public Task<List<ExpenseEntry>> GetByCategoryAsync(int userId, int categoryId) =>
        _repo.GetByUserIdAndCategoryAsync(userId, categoryId);

    public Task<List<ExpenseEntry>> SearchExpenseEntriesAsync(int userId, string keyword) =>
        _repo.SearchByDescriptionAsync(userId, keyword);

    public Task<decimal> GetTotalByUserAsync(int userId) =>
        _repo.SumByUserIdAsync(userId);

    public Task<decimal> GetTotalByDateRangeAsync(int userId, DateTime start, DateTime end) =>
        _repo.SumByUserIdAndDateRangeAsync(userId, start, end);

    public Task<List<ExpenseEntry>> GetRecurringExpenseEntriesAsync(int userId) =>
        _repo.GetRecurringAsync(userId);

    public async Task UpdateExpenseEntryAsync(int id, UpdateExpenseEntryDto dto)
    {
        var expenseEntry = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"ExpenseEntry {id} not found.");

        if (dto.Amount.HasValue) expenseEntry.Amount = dto.Amount.Value;
        if (dto.Description != null) expenseEntry.Description = dto.Description;
        if (dto.Date.HasValue) expenseEntry.Date = dto.Date.Value;
        if (dto.PaymentMode != null) expenseEntry.PaymentMode = dto.PaymentMode;
        if (dto.Tags != null) expenseEntry.Tags = dto.Tags;
        if (dto.IsRecurring.HasValue) expenseEntry.IsRecurring = dto.IsRecurring.Value;

        expenseEntry.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateAsync(expenseEntry);
        await _repo.SaveChangesAsync();
    }

    public async Task DeleteExpenseEntryAsync(int id)
    {
        var expenseEntry = await _repo.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"ExpenseEntry {id} not found.");
        await _repo.DeleteAsync(expenseEntry);
        await _repo.SaveChangesAsync();
    }
}