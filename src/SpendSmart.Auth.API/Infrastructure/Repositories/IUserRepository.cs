using SpendSmart.Auth.Domain.Entities;

namespace SpendSmart.Auth.Infrastructure.Repositories;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email);
    Task<User?> FindByUserIdAsync(int userId);
    Task<bool> ExistsByEmailAsync(string email);
    Task<List<User>> FindAllActiveAsync();
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task UpdateLastLoginAsync(int userId, DateTime loginTime);
    Task UpdateCurrencyAsync(int userId, string currency);
    Task SaveChangesAsync();
    
    Task<User?> FindByGoogleIdAsync(string googleId);
    Task<User?> FindByGoogleIdOrEmailAsync(string googleId, string email);
    Task UpdateGoogleTokensAsync(int userId, string? accessToken, string? refreshToken, DateTime? expiry);
}