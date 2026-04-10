using Microsoft.EntityFrameworkCore;
using SpendSmart.Category.API.Domain.Entities;

namespace SpendSmart.Category.API.Infrastructure.Data;

public class CategoryDbContext : DbContext
{
    public CategoryDbContext(DbContextOptions<CategoryDbContext> options) : base(options) { }

    public DbSet<CategoryItem> CategoryItems => Set<CategoryItem>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<CategoryItem>(e =>
        {
            e.HasKey(c => c.CategoryItemId);
            e.HasIndex(c => new { c.UserId, c.Name }).IsUnique();
            e.HasIndex(c => new { c.UserId, c.IsActive });
            e.HasIndex(c => new { c.IsDefault, c.IsActive });
            e.Property(c => c.Name).HasMaxLength(50).IsRequired();
            e.Property(c => c.Icon).HasMaxLength(10);
            e.Property(c => c.Color).HasMaxLength(20);
            e.Property(c => c.Type).HasMaxLength(20).HasDefaultValue("EXPENSE");
        });
    }
}