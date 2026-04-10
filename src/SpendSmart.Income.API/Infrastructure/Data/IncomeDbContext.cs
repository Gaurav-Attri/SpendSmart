using Microsoft.EntityFrameworkCore;
using SpendSmart.Income.API.Domain.Entities;

namespace SpendSmart.Income.API.Infrastructure.Data;

public class IncomeDbContext : DbContext
{
    public IncomeDbContext(DbContextOptions<IncomeDbContext> options) : base(options) { }

    public DbSet<IncomeRecord> IncomeRecords => Set<IncomeRecord>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<IncomeRecord>(e =>
        {
            e.HasKey(x => x.IncomeRecordId);
            e.HasIndex(x => new { x.UserId, x.Date });
            e.HasIndex(x => new { x.UserId, x.Source });
            e.HasIndex(x => new { x.UserId, x.IsRecurring });
            e.Property(x => x.Amount).HasPrecision(18, 2);
            e.Property(x => x.Currency).HasMaxLength(3).HasDefaultValue("INR");
            e.Property(x => x.Source).HasMaxLength(50).HasDefaultValue("OTHER");
            e.Property(x => x.Description).HasMaxLength(500);
            e.Property(x => x.RecurrenceType).HasMaxLength(20);
        });
    }
}