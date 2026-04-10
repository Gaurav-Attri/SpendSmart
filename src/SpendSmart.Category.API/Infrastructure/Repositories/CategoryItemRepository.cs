using Microsoft.EntityFrameworkCore;
using SpendSmart.Category.API.Domain.Entities;
using SpendSmart.Category.API.Infrastructure.Data;

namespace SpendSmart.Category.API.Infrastructure.Repositories;

public class CategoryItemRepository : ICategoryItemRepository
{
    private readonly CategoryDbContext _ctx;

    public CategoryItemRepository(CategoryDbContext ctx)
    {
        _ctx = ctx;
    }

    public Task<CategoryItem?> GetByIdAsync(int id) =>
        _ctx.CategoryItems.FindAsync(id).AsTask();

    public Task<List<CategoryItem>> GetAllForUserAsync(int userId) =>
        _ctx.CategoryItems
            .Where(c => (c.UserId == null || c.UserId == userId) && c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public Task<List<CategoryItem>> GetDefaultsAsync() =>
        _ctx.CategoryItems
            .Where(c => c.IsDefault && c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();

    public Task<bool> ExistsByNameAndUserAsync(int userId, string name) =>
        _ctx.CategoryItems
            .AnyAsync(c => c.UserId == userId && c.Name == name);

    public Task<bool> ExistsDefaultByNameAsync(string name) =>
        _ctx.CategoryItems
            .AnyAsync(c => c.IsDefault && c.Name == name);

    public async Task AddAsync(CategoryItem categoryItem) =>
        await _ctx.CategoryItems.AddAsync(categoryItem);

    public Task UpdateAsync(CategoryItem categoryItem)
    {
        _ctx.CategoryItems.Update(categoryItem);
        return Task.CompletedTask;
    }

    public Task DeactivateAsync(int categoryItemId) =>
        _ctx.CategoryItems
            .Where(c => c.CategoryItemId == categoryItemId)
            .ExecuteUpdateAsync(s => s.SetProperty(c => c.IsActive, false));

    public Task SaveChangesAsync() =>
        _ctx.SaveChangesAsync();
}