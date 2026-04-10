using System.ComponentModel.DataAnnotations;

namespace SpendSmart.Category.API.Application.DTOs;

public class CreateCategoryItemDto
{
    public int? UserId { get; set; }

    [Required, StringLength(50, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(10)]
    public string Icon { get; set; } = "📁";

    [StringLength(20)]
    [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be a valid hex code like #4CAF50")]
    public string Color { get; set; } = "#4CAF50";

    [RegularExpression(@"^(EXPENSE|INCOME)$", ErrorMessage = "Type must be EXPENSE or INCOME")]
    public string Type { get; set; } = "EXPENSE";

    public bool IsDefault { get; set; } = false;
}