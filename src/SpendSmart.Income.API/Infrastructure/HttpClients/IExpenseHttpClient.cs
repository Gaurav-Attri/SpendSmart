namespace SpendSmart.Income.API.Infrastructure.HttpClients;

public interface IExpenseHttpClient
{
    Task<decimal> GetTotalByUserAsync(int userId);
}