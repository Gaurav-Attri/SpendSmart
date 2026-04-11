using Microsoft.EntityFrameworkCore;
using SpendSmart.Budget.API.Domain.Entities;

namespace SpendSmart.Budget.API.Infrastructure.Data;

public class BudgetDbContext : DbContext
{
    public BudgetDbContext(DbContextOptions<BudgetDbContext> options) : base(options) { }

    public DbSet<BudgetPlan> BudgetPlans => Set<BudgetPlan>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<BudgetPlan>(e =>
        {
            e.HasKey(b => b.BudgetPlanId);
            e.HasIndex(b => new { b.UserId, b.CategoryId, b.Period }).IsUnique();
            e.HasIndex(b => new { b.UserId, b.IsActive });
            e.Property(b => b.LimitAmount).HasPrecision(18, 2);
            e.Property(b => b.SpentAmount).HasPrecision(18, 2).HasDefaultValue(0);
            e.Property(b => b.Currency).HasMaxLength(3).HasDefaultValue("INR");
            e.Property(b => b.Period).HasMaxLength(20).HasDefaultValue("MONTHLY");
            e.Property(b => b.Name).HasMaxLength(100);
        });
    }
}