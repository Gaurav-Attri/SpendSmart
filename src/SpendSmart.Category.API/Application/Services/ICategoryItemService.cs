using SpendSmart.Category.API.Application.DTOs;
using SpendSmart.Category.API.Domain.Entities;

namespace SpendSmart.Category.API.Application.Services;

public interface ICategoryItemService
{
    Task SeedDefaultCategoriesAsync(int userId);
    Task<CategoryItem> CreateCustomCategoryAsync(CreateCategoryItemDto dto);
    Task<List<CategoryItem>> GetAllForUserAsync(int userId);
    Task<List<CategoryItem>> GetDefaultsAsync();
    Task<CategoryItem?> GetByIdAsync(int id);
    Task DeactivateCategoryAsync(int categoryItemId);
}