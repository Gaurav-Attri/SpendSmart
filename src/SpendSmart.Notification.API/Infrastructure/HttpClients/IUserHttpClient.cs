namespace SpendSmart.Notification.API.Infrastructure.HttpClients;

public interface IUserHttpClient
{
    Task<List<int>> GetAllActiveUserIdsAsync();
    Task<string?> GetUserEmailAsync(int userId);
}