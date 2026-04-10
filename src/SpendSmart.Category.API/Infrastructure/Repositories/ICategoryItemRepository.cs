using SpendSmart.Category.API.Domain.Entities;

namespace SpendSmart.Category.API.Infrastructure.Repositories;

public interface ICategoryItemRepository
{
    Task<CategoryItem?> GetByIdAsync(int id);
    Task<List<CategoryItem>> GetAllForUserAsync(int userId);
    Task<List<CategoryItem>> GetDefaultsAsync();
    Task<bool> ExistsByNameAndUserAsync(int userId, string name);
    Task<bool> ExistsDefaultByNameAsync(string name);
    Task AddAsync(CategoryItem categoryItem);
    Task UpdateAsync(CategoryItem categoryItem);
    Task DeactivateAsync(int categoryItemId);
    Task SaveChangesAsync();
}