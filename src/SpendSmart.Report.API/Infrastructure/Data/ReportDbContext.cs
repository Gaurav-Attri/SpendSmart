using Microsoft.EntityFrameworkCore;
using SpendSmart.Report.API.Domain.Entities;

namespace SpendSmart.Report.API.Infrastructure.Data;

public class ReportDbContext : DbContext
{
    public ReportDbContext(DbContextOptions<ReportDbContext> options) : base(options) { }

    public DbSet<ReportRecord> ReportRecords => Set<ReportRecord>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<ReportRecord>(e =>
        {
            e.HasKey(r => r.ReportRecordId);
            e.HasIndex(r => new { r.UserId, r.GeneratedAt });
            e.HasIndex(r => r.ReportType);
            e.Property(r => r.Title).HasMaxLength(200);
            e.Property(r => r.FilePath).HasMaxLength(500);
            e.Property(r => r.Status).HasMaxLength(20).HasDefaultValue("GENERATED");
            e.Property(r => r.ReportType).HasMaxLength(50).HasDefaultValue("MONTHLY");
        });
    }
}