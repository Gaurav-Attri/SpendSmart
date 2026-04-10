namespace SpendSmart.Category.API.Domain.Entities;

public class CategoryItem
{
    public int CategoryItemId { get; set; }
    public int? UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Icon { get; set; } = "📁";
    public string Color { get; set; } = "#4CAF50";
    public string Type { get; set; } = "EXPENSE";
    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}