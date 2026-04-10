using SpendSmart.Category.API.Application.DTOs;
using SpendSmart.Category.API.Domain.Entities;
using SpendSmart.Category.API.Infrastructure.Repositories;

namespace SpendSmart.Category.API.Application.Services;

public class CategoryItemService : ICategoryItemService
{
    private readonly ICategoryItemRepository _repo;

    public CategoryItemService(ICategoryItemRepository repo)
    {
        _repo = repo;
    }

    public async Task SeedDefaultCategoriesAsync(int userId)
    {
        var defaults = new[]
        {
            new { Name = "Food", Icon = "🍔", Color = "#F44336", Type = "EXPENSE" },
            new { Name = "Transport", Icon = "🚗", Color = "#2196F3", Type = "EXPENSE" },
            new { Name = "Entertainment", Icon = "🎬", Color = "#9C27B0", Type = "EXPENSE" },
            new { Name = "Health", Icon = "🏥", Color = "#E91E63", Type = "EXPENSE" },
            new { Name = "Shopping", Icon = "🛒", Color = "#FF9800", Type = "EXPENSE" },
            new { Name = "Bills", Icon = "📄", Color = "#607D8B", Type = "EXPENSE" },
            new { Name = "Education", Icon = "📚", Color = "#3F51B5", Type = "EXPENSE" },
            new { Name = "Salary", Icon = "💰", Color = "#4CAF50", Type = "INCOME" },
            new { Name = "Freelance", Icon = "💻", Color = "#00BCD4", Type = "INCOME" },
            new { Name = "Investment", Icon = "📈", Color = "#8BC34A", Type = "INCOME" }
        };

        foreach (var def in defaults)
        {
            if (!await _repo.ExistsByNameAndUserAsync(userId, def.Name))
            {
                await _repo.AddAsync(new CategoryItem
                {
                    UserId = userId,
                    Name = def.Name,
                    Icon = def.Icon,
                    Color = def.Color,
                    Type = def.Type,
                    IsDefault = true,
                    IsActive = true
                });
            }
        }
        await _repo.SaveChangesAsync();
    }

    public async Task<CategoryItem> CreateCustomCategoryAsync(CreateCategoryItemDto dto)
    {
        var categoryItem = new CategoryItem
        {
            UserId = dto.UserId,
            Name = dto.Name,
            Icon = dto.Icon,
            Color = dto.Color,
            Type = dto.Type,
            IsDefault = dto.IsDefault,
            IsActive = true
        };

        await _repo.AddAsync(categoryItem);
        await _repo.SaveChangesAsync();
        return categoryItem;
    }

    public Task<List<CategoryItem>> GetAllForUserAsync(int userId) =>
        _repo.GetAllForUserAsync(userId);

    public Task<List<CategoryItem>> GetDefaultsAsync() =>
        _repo.GetDefaultsAsync();

    public Task<CategoryItem?> GetByIdAsync(int id) =>
        _repo.GetByIdAsync(id);

    public async Task DeactivateCategoryAsync(int categoryItemId)
    {
        await _repo.DeactivateAsync(categoryItemId);
        await _repo.SaveChangesAsync();
    }
}