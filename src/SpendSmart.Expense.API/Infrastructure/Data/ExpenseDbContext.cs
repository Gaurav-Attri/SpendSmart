using Microsoft.EntityFrameworkCore;
using SpendSmart.Expense.API.Domain.Entities;

namespace SpendSmart.Expense.API.Infrastructure.Data;

public class ExpenseDbContext : DbContext
{
    public ExpenseDbContext(DbContextOptions<ExpenseDbContext> options) : base(options) { }

    public DbSet<ExpenseEntry> ExpenseEntries => Set<ExpenseEntry>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<ExpenseEntry>(e =>
        {
            e.HasKey(x => x.ExpenseEntryId);
            e.HasIndex(x => new { x.UserId, x.Date });
            e.HasIndex(x => new { x.UserId, x.CategoryId });
            e.HasIndex(x => new { x.UserId, x.IsRecurring });
            e.Property(x => x.Amount).HasPrecision(18, 2);
            e.Property(x => x.PaymentMode).HasMaxLength(20).HasDefaultValue("CASH");
            e.Property(x => x.Currency).HasMaxLength(3).HasDefaultValue("INR");
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.Tags).HasMaxLength(200);
            e.Property(x => x.ReceiptUrl).HasMaxLength(500);
        });
    }
}