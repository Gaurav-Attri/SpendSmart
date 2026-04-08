using Microsoft.EntityFrameworkCore;
using SpendSmart.Auth.Domain.Entities;
using SpendSmart.Auth.Infrastructure.Data;

namespace SpendSmart.Auth.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _ctx;

    public UserRepository(AuthDbContext ctx)
    {
        _ctx = ctx;
    }

    public Task<User?> FindByEmailAsync(string email) =>
        _ctx.Users.FirstOrDefaultAsync(u => u.Email == email);

    public Task<User?> FindByUserIdAsync(int id) =>
        _ctx.Users.FindAsync(id).AsTask();

    public Task<bool> ExistsByEmailAsync(string email) =>
        _ctx.Users.AnyAsync(u => u.Email == email);

    public Task<List<User>> FindAllActiveAsync() =>
        _ctx.Users.Where(u => u.IsActive).ToListAsync();

    public async Task AddAsync(User user) =>
        await _ctx.Users.AddAsync(user);

    public Task UpdateAsync(User user)
    {
        _ctx.Users.Update(user);
        return Task.CompletedTask;
    }

    public Task UpdateLastLoginAsync(int userId, DateTime loginTime) =>
        _ctx.Users.Where(u => u.UserId == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.LastLoginAt, loginTime));

    public Task UpdateCurrencyAsync(int userId, string currency) =>
        _ctx.Users.Where(u => u.UserId == userId)
            .ExecuteUpdateAsync(s => s.SetProperty(u => u.Currency, currency));

    public Task SaveChangesAsync() =>
        _ctx.SaveChangesAsync();

    public Task<User?> FindByGoogleIdAsync(string googleId) =>
        _ctx.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId);

    public Task<User?> FindByGoogleIdOrEmailAsync(string googleId, string email) =>
        _ctx.Users.FirstOrDefaultAsync(u => u.GoogleId == googleId || u.Email == email);

    public Task UpdateGoogleTokensAsync(int userId, string? accessToken, string? refreshToken, DateTime? expiry) =>
        _ctx.Users.Where(u => u.UserId == userId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(u => u.GoogleAccessToken, accessToken)
                .SetProperty(u => u.GoogleRefreshToken, refreshToken)
                .SetProperty(u => u.GoogleTokenExpiry, expiry));
}