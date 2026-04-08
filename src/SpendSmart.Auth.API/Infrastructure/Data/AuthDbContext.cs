using Microsoft.EntityFrameworkCore;
using SpendSmart.Auth.Domain.Entities;

namespace SpendSmart.Auth.Infrastructure.Data;

public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<User>(e =>
        {
            e.HasKey(u => u.UserId);
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.GoogleId).IsUnique();
            e.Property(u => u.Currency).HasMaxLength(3).HasDefaultValue("INR");
            e.Property(u => u.PasswordHash).IsRequired().HasMaxLength(256);
            e.Property(u => u.Role).HasMaxLength(20).HasDefaultValue("User");
            e.Property(u => u.GoogleId).HasMaxLength(100);
            e.Property(u => u.FullName).HasMaxLength(100).IsRequired();
        });

        mb.Entity<AuditLog>(e =>
        {
            e.HasKey(a => a.AuditLogId);
            e.HasIndex(a => a.ActorUserId);
            e.Property(a => a.Action).HasMaxLength(100);
            e.Property(a => a.EntityName).HasMaxLength(100);
        });
    }
}